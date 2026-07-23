-- ADO.NET Library System - Database Schema

CREATE TABLE members (
    member_id INTEGER PRIMARY KEY,
    first_name TEXT NOT NULL,
    last_name TEXT NOT NULL,
    email TEXT NOT NULL UNIQUE,
    membership_date TEXT NOT NULL
);

CREATE TABLE books (
    book_id INTEGER PRIMARY KEY,
    title TEXT NOT NULL,
    author TEXT NOT NULL,
    isbn TEXT NOT NULL UNIQUE,
    available_copies INTEGER NOT NULL CHECK (available_copies >= 0)
);

CREATE TABLE loans (
    loan_id INTEGER PRIMARY KEY,
    member_id INTEGER NOT NULL,
    book_id INTEGER NOT NULL,
    loan_date TEXT NOT NULL,
    return_date TEXT,
    FOREIGN KEY (member_id) REFERENCES members(member_id),
    FOREIGN KEY (book_id) REFERENCES books(book_id)
);

INSERT INTO members VALUES
(1, 'Ahmet', 'Yilmaz', 'ahmet@mail.com', '2024-01-15'),
(2, 'Ayse', 'Kaya', 'ayse@mail.com', '2024-02-20'),
(3, 'Mehmet', 'Demir', 'mehmet@mail.com', '2024-03-10');

INSERT INTO books VALUES
(1, 'Clean Code', 'Robert Martin', '9780132350884', 3),
(2, 'The Pragmatic Programmer', 'Andrew Hunt', '9780201616224', 2),
(3, 'Design Patterns', 'Gang of Four', '9780201633610', 1);

INSERT INTO loans VALUES
(1, 1, 1, '2024-06-01', NULL),
(2, 2, 2, '2024-06-05', '2024-06-20'),
(3, 1, 3, '2024-06-10', NULL);
