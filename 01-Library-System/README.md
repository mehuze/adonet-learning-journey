# ADO.NET Library System — Data Access Layer

## 🇬🇧 English

### Overview
Imagine you are building a library management system. Your C# application needs to talk to a database — read books, add new ones, record loans. But C# speaks in objects, and the database speaks in tables. They need a translator. That translator is ADO.NET.

This project builds a complete data access layer using ADO.NET, covering every core concept: connections, commands, parameters, readers, and transactions.

### What is ADO.NET? (The Restaurant Analogy)
Think of a restaurant:
- **You** = the C# application (the customer)
- **The kitchen** = the database (where the data lives)
- **ADO.NET** = the waiter (the bridge between you and the kitchen)

You tell the waiter what you want (SQL query), the waiter goes to the kitchen, and brings back your food (data) neatly arranged on a plate (objects).

### Core Concepts Demonstrated

**1. DbConnection — The Phone Call**
A connection is like a phone call to the database. You dial (Open), talk (run queries), and hang up (Close). You never leave the line open — databases have a limited number of connections, and leaving one open blocks others.

```csharp
using var connection = dbFactory.CreateConnection();
connection.ConnectionString = connectionString;
connection.Open();
// ... work here ...
// using closes it automatically, even if an error is thrown
```

The `using` keyword guarantees the connection closes — even if an exception occurs mid-way. Without it, a crash could leave connections open forever (a "connection leak").

**2. DbCommand — Placing the Order**
The command carries your SQL instruction to the database. It needs two things: what to run (CommandText) and where to run it (Connection).

**3. DbParameter — Safety First**
Never build SQL by gluing strings together. Parameters keep values separate from the query structure, which prevents SQL Injection attacks.

```csharp
// UNSAFE - never do this:
command.CommandText = "SELECT * FROM books WHERE id = " + userInput;

// SAFE - always do this:
command.CommandText = "SELECT * FROM books WHERE id = @BookId";
parameter.Value = userInput;
```

**4. The Three Execute Methods**

| Method | When to use | Returns |
|---|---|---|
| `ExecuteReader()` | SELECT returning many rows | DbDataReader (row by row) |
| `ExecuteScalar()` | SELECT returning one value (COUNT, SUM) | object (single value) |
| `ExecuteNonQuery()` | INSERT, UPDATE, DELETE | int (rows affected) |

Think of it as ordering food:
- **Reader** = a full table of dishes (many rows)
- **Scalar** = a single item (one value)
- **NonQuery** = telling the kitchen to change something (no food returned, just a confirmation)

**5. DbTransaction — All or Nothing**
Borrowing a book requires two steps: create a loan record AND decrease the stock count. If the second step fails, the first must be undone — otherwise you'd have a loan for a book that was never taken off the shelf.

A transaction wraps both steps together:
- Both succeed → `Commit()` makes them permanent
- Either fails → `Rollback()` undoes everything

This is the "A" in ACID — **Atomicity**.

### Methods Implemented

| Method | Concept Demonstrated |
|---|---|
| `GetAllBooks()` | ExecuteReader — reading many rows |
| `GetBookById(int)` | DbParameter — safe parameterized query |
| `GetTotalBookCount()` | ExecuteScalar — single value |
| `AddBook(Book)` | INSERT + retrieving generated ID |
| `UpdateBookCopies(int, int)` | ExecuteNonQuery — UPDATE |
| `DeleteBook(int)` | ExecuteNonQuery — DELETE |
| `BorrowBook(int, int, DateTime)` | **Transaction** — commit/rollback |
| `GetActiveLoans()` | INNER JOIN across three tables |

### Database Schema
- **members**: member_id (PK), first_name, last_name, email (UNIQUE), membership_date
- **books**: book_id (PK), title, author, isbn (UNIQUE), available_copies (CHECK >= 0)
- **loans**: loan_id (PK), member_id (FK), book_id (FK), loan_date, return_date

### What I Learned
- How ADO.NET bridges C# objects and database tables
- Why connections must be opened late and closed early
- How `using` guarantees cleanup even when exceptions occur
- Why parameters are essential for security (SQL Injection prevention)
- The difference between the three Execute methods and when to use each
- How transactions guarantee data consistency (all or nothing)
- Using `DbProviderFactory` to stay database-agnostic

### Technologies Used
C#, ADO.NET, SQLite, SQL

---

## 🇹🇷 Türkçe

### Genel Bakış
Bir kütüphane yönetim sistemi kurduğunu hayal et. C# uygulamanın veritabanıyla konuşması gerekiyor — kitapları okumak, yenilerini eklemek, ödünç kayıtlarını tutmak. Ama C# nesnelerle konuşur, veritabanı tablolarla. Aralarında bir tercümana ihtiyaç var. İşte o tercüman ADO.NET.

Bu proje, ADO.NET'in tüm temel kavramlarını kapsayan eksiksiz bir veri erişim katmanı kuruyor: bağlantılar, komutlar, parametreler, okuyucular ve transaction'lar.

### ADO.NET Nedir? (Restoran Benzetmesi)
Bir restoran düşün:
- **Sen** = C# uygulaması (müşteri)
- **Mutfak** = veritabanı (verilerin durduğu yer)
- **ADO.NET** = garson (aranızdaki köprü)

Garsona ne istediğini söylersin (SQL sorgusu), garson mutfağa gider ve yemeğini (veriyi) düzenli bir tabakta (nesne olarak) geri getirir.

### Gösterilen Temel Kavramlar

**1. DbConnection — Telefon Görüşmesi**
Bağlantı, veritabanına yapılan bir telefon görüşmesi gibidir. Numarayı çevirirsin (Open), konuşursun (sorgu çalıştırırsın), telefonu kaparsın (Close). Hattı asla açık bırakmazsın — veritabanlarının sınırlı sayıda bağlantısı vardır, birini açık bırakmak diğerlerini engeller.

`using` anahtar kelimesi bağlantının kapanmasını **garanti eder** — yarı yolda bir hata (exception) çıksa bile. Onsuz, bir çökme bağlantıları sonsuza kadar açık bırakabilir (buna "connection leak" denir).

**2. DbCommand — Siparişi Vermek**
Komut, SQL talimatını veritabanına taşır. İki şeye ihtiyacı vardır: **ne** çalıştırılacak (CommandText) ve **nerede** çalıştırılacak (Connection).

**3. DbParameter — Önce Güvenlik**
SQL'i asla string birleştirerek kurma. Parametreler değerleri sorgu yapısından ayrı tutar, bu da SQL Injection saldırılarını engeller.

```csharp
// GÜVENSİZ - asla böyle yapma:
command.CommandText = "SELECT * FROM books WHERE id = " + kullaniciGirdisi;

// GÜVENLİ - her zaman böyle yap:
command.CommandText = "SELECT * FROM books WHERE id = @BookId";
parameter.Value = kullaniciGirdisi;
```

**4. Üç Execute Metodu (3 Kardeş)**

| Metot | Ne zaman? | Ne döner? |
|---|---|---|
| `ExecuteReader()` | Çok satır dönen SELECT | DbDataReader (satır satır) |
| `ExecuteScalar()` | Tek değer dönen SELECT (COUNT, SUM) | object (tek değer) |
| `ExecuteNonQuery()` | INSERT, UPDATE, DELETE | int (etkilenen satır sayısı) |

Yemek siparişi gibi düşün:
- **Reader** = dolu bir sofra (çok satır)
- **Scalar** = tek bir tabak (tek değer)
- **NonQuery** = mutfağa bir şey değiştirmesini söylemek (yemek gelmez, sadece onay)

**5. DbTransaction — Ya Hep Ya Hiç**
Kitap ödünç vermek iki adım gerektirir: ödünç kaydı oluştur VE stok sayısını azalt. İkinci adım başarısız olursa, birincisi de geri alınmalı — yoksa raftan hiç alınmamış bir kitap için ödünç kaydın olur.

Transaction ikisini birlikte sarar:
- İkisi de başarılı → `Commit()` kalıcı yapar
- Biri başarısız → `Rollback()` her şeyi geri alır

Bu, ACID'in "A"sıdır — **Atomicity (Bölünmezlik)**.

### Yazılan Metotlar

| Metot | Gösterdiği Kavram |
|---|---|
| `GetAllBooks()` | ExecuteReader — çok satır okuma |
| `GetBookById(int)` | DbParameter — güvenli parametreli sorgu |
| `GetTotalBookCount()` | ExecuteScalar — tek değer |
| `AddBook(Book)` | INSERT + üretilen ID'yi alma |
| `UpdateBookCopies(int, int)` | ExecuteNonQuery — UPDATE |
| `DeleteBook(int)` | ExecuteNonQuery — DELETE |
| `BorrowBook(int, int, DateTime)` | **Transaction** — commit/rollback |
| `GetActiveLoans()` | Üç tablo arası INNER JOIN |

### Veritabanı Şeması
- **members**: member_id (PK), first_name, last_name, email (UNIQUE), membership_date
- **books**: book_id (PK), title, author, isbn (UNIQUE), available_copies (CHECK >= 0)
- **loans**: loan_id (PK), member_id (FK), book_id (FK), loan_date, return_date

### Neler Öğrendim?
- ADO.NET'in C# nesneleri ile veritabanı tabloları arasında nasıl köprü kurduğunu
- Bağlantıların neden geç açılıp erken kapatılması gerektiğini
- `using`'in hata olsa bile temizliği nasıl garanti ettiğini
- Parametrelerin güvenlik için neden şart olduğunu (SQL Injection önleme)
- Üç Execute metodu arasındaki farkı ve hangisinin ne zaman kullanılacağını
- Transaction'ların veri tutarlılığını nasıl garanti ettiğini (ya hep ya hiç)
- `DbProviderFactory` ile veritabanı bağımsız kod yazmayı

### Kullanılan Teknolojiler
C#, ADO.NET, SQLite, SQL
