using System.Data.Common;

namespace LibrarySystem;

/// <summary>
/// ADO.NET ile kutuphane veritabani islemleri.
/// Bagimsiz calisan bir veri erisim katmani (Data Access Layer).
/// </summary>
public class LibraryService
{
    private readonly DbProviderFactory dbFactory;
    private readonly string connectionString;

    public LibraryService(DbProviderFactory dbFactory, string connectionString)
    {
        ArgumentNullException.ThrowIfNull(dbFactory);
        ArgumentNullException.ThrowIfNull(connectionString);

        this.dbFactory = dbFactory;
        this.connectionString = connectionString;
    }

    // ============================================================
    // 1) ExecuteReader ORNEGI - Cok satir okuma
    // ============================================================
    public List<Book> GetAllBooks()
    {
        using var connection = this.dbFactory.CreateConnection()!;
        connection.ConnectionString = this.connectionString;
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT book_id, title, author, isbn, available_copies FROM books";

        var books = new List<Book>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            books.Add(new Book
            {
                Id = reader.GetInt32(0),
                Title = reader.GetString(1),
                Author = reader.GetString(2),
                Isbn = reader.GetString(3),
                AvailableCopies = reader.GetInt32(4),
            });
        }

        return books;
    }

    // ============================================================
    // 2) DbParameter ORNEGI - Guvenli parametre (SQL Injection korumasi)
    // ============================================================
    public Book? GetBookById(int bookId)
    {
        using var connection = this.dbFactory.CreateConnection()!;
        connection.ConnectionString = this.connectionString;
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT book_id, title, author, isbn, available_copies FROM books WHERE book_id = @BookId";

        var parameter = command.CreateParameter();
        parameter.ParameterName = "@BookId";
        parameter.Value = bookId;
        command.Parameters.Add(parameter);

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Book
            {
                Id = reader.GetInt32(0),
                Title = reader.GetString(1),
                Author = reader.GetString(2),
                Isbn = reader.GetString(3),
                AvailableCopies = reader.GetInt32(4),
            };
        }

        return null;
    }

    // ============================================================
    // 3) ExecuteScalar ORNEGI - Tek deger dondurme
    // ============================================================
    public int GetTotalBookCount()
    {
        using var connection = this.dbFactory.CreateConnection()!;
        connection.ConnectionString = this.connectionString;
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM books";

        var result = command.ExecuteScalar();
        return Convert.ToInt32(result);
    }

    // ============================================================
    // 4) ExecuteNonQuery ORNEGI - INSERT (veri degistirme)
    // ============================================================
    public int AddBook(Book book)
    {
        ArgumentNullException.ThrowIfNull(book);

        using var connection = this.dbFactory.CreateConnection()!;
        connection.ConnectionString = this.connectionString;
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"INSERT INTO books (title, author, isbn, available_copies)
                                VALUES (@Title, @Author, @Isbn, @Copies);
                                SELECT last_insert_rowid();";

        AddParameter(command, "@Title", book.Title);
        AddParameter(command, "@Author", book.Author);
        AddParameter(command, "@Isbn", book.Isbn);
        AddParameter(command, "@Copies", book.AvailableCopies);

        var newId = command.ExecuteScalar();
        return Convert.ToInt32(newId);
    }

    // ============================================================
    // 5) ExecuteNonQuery ORNEGI - UPDATE
    // ============================================================
    public bool UpdateBookCopies(int bookId, int newCopies)
    {
        using var connection = this.dbFactory.CreateConnection()!;
        connection.ConnectionString = this.connectionString;
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "UPDATE books SET available_copies = @Copies WHERE book_id = @BookId";

        AddParameter(command, "@Copies", newCopies);
        AddParameter(command, "@BookId", bookId);

        int affectedRows = command.ExecuteNonQuery();
        return affectedRows > 0;
    }

    // ============================================================
    // 6) ExecuteNonQuery ORNEGI - DELETE
    // ============================================================
    public bool DeleteBook(int bookId)
    {
        using var connection = this.dbFactory.CreateConnection()!;
        connection.ConnectionString = this.connectionString;
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM books WHERE book_id = @BookId";

        AddParameter(command, "@BookId", bookId);

        int affectedRows = command.ExecuteNonQuery();
        return affectedRows > 0;
    }

    // ============================================================
    // 7) TRANSACTION ORNEGI - Ya hep ya hic
    // Kitap odunc verme: hem loan kaydi ekle, hem stok azalt.
    // Biri basarisiz olursa ikisi de geri alinir (rollback).
    // ============================================================
    public bool BorrowBook(int memberId, int bookId, DateTime loanDate)
    {
        using var connection = this.dbFactory.CreateConnection()!;
        connection.ConnectionString = this.connectionString;
        connection.Open();

        using var transaction = connection.BeginTransaction();

        try
        {
            // Adim 1: Odunc kaydi ekle
            using (var insertCommand = connection.CreateCommand())
            {
                insertCommand.Transaction = transaction;
                insertCommand.CommandText = @"INSERT INTO loans (member_id, book_id, loan_date, return_date)
                                              VALUES (@MemberId, @BookId, @LoanDate, NULL)";
                AddParameter(insertCommand, "@MemberId", memberId);
                AddParameter(insertCommand, "@BookId", bookId);
                AddParameter(insertCommand, "@LoanDate", loanDate.ToString("yyyy-MM-dd"));
                insertCommand.ExecuteNonQuery();
            }

            // Adim 2: Stoktan bir adet dus
            using (var updateCommand = connection.CreateCommand())
            {
                updateCommand.Transaction = transaction;
                updateCommand.CommandText = @"UPDATE books SET available_copies = available_copies - 1
                                              WHERE book_id = @BookId AND available_copies > 0";
                AddParameter(updateCommand, "@BookId", bookId);

                int updated = updateCommand.ExecuteNonQuery();
                if (updated == 0)
                {
                    // Stok yok - her seyi geri al
                    transaction.Rollback();
                    return false;
                }
            }

            // Ikisi de basarili - kalici yap
            transaction.Commit();
            return true;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    // ============================================================
    // 8) JOIN ile rapor - Aktif odunc alinan kitaplar
    // ============================================================
    public List<LoanReport> GetActiveLoans()
    {
        using var connection = this.dbFactory.CreateConnection()!;
        connection.ConnectionString = this.connectionString;
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"SELECT m.first_name, m.last_name, b.title, l.loan_date
                                FROM loans l
                                INNER JOIN members m ON l.member_id = m.member_id
                                INNER JOIN books b ON l.book_id = b.book_id
                                WHERE l.return_date IS NULL
                                ORDER BY l.loan_date";

        var reports = new List<LoanReport>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            reports.Add(new LoanReport
            {
                MemberName = $"{reader.GetString(0)} {reader.GetString(1)}",
                BookTitle = reader.GetString(2),
                LoanDate = reader.GetString(3),
            });
        }

        return reports;
    }

    // Yardimci metot - parametre ekleme islemini kisaltir
    private static void AddParameter(DbCommand command, string name, object? value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value ?? DBNull.Value;
        command.Parameters.Add(parameter);
    }
}

public class Book
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Author { get; set; } = string.Empty;

    public string Isbn { get; set; } = string.Empty;

    public int AvailableCopies { get; set; }
}

public class LoanReport
{
    public string MemberName { get; set; } = string.Empty;

    public string BookTitle { get; set; } = string.Empty;

    public string LoanDate { get; set; } = string.Empty;
}
