# ADO.NET Learning Journey

A collection of ADO.NET practice projects covering data access patterns, database connections, commands, parameters, readers, and transactions in C#.

## 📚 Projects

| # | Project | Concepts Covered |
|---|---|---|
| 01 | [Library System](01-Library-System) | DbConnection, DbCommand, DbParameter, ExecuteReader/Scalar/NonQuery, DbTransaction, INNER JOIN |

## 🎯 What is ADO.NET?

ADO.NET is the data access technology in .NET that lets your C# application communicate with databases. Think of it as a waiter in a restaurant — you (the application) place an order (SQL query), the waiter (ADO.NET) takes it to the kitchen (database), and brings back your food (data) neatly arranged.

## 🔑 Core Concepts

- **DbConnection** — opens and closes the communication channel to the database
- **DbCommand** — carries the SQL instruction to be executed
- **DbParameter** — passes values safely, preventing SQL Injection
- **DbDataReader** — reads query results row by row
- **DbTransaction** — groups operations so they all succeed or all fail together
- **DbProviderFactory** — keeps code database-agnostic (SQL Server, SQLite, MySQL...)

---

# ADO.NET Öğrenme Yolculuğu

C#'ta veri erişim desenleri, veritabanı bağlantıları, komutlar, parametreler, okuyucular ve transaction'ları kapsayan ADO.NET pratik projeleri koleksiyonu.

## 📚 Projeler

| # | Proje | İşlenen Kavramlar |
|---|---|---|
| 01 | [Kütüphane Sistemi](01-Library-System) | DbConnection, DbCommand, DbParameter, ExecuteReader/Scalar/NonQuery, DbTransaction, INNER JOIN |

## 🎯 ADO.NET Nedir?

ADO.NET, C# uygulamanızın veritabanlarıyla iletişim kurmasını sağlayan .NET veri erişim teknolojisidir. Restorandaki bir garson gibi düşünün — siz (uygulama) siparişinizi verirsiniz (SQL sorgusu), garson (ADO.NET) bunu mutfağa (veritabanı) götürür ve yemeğinizi (veriyi) düzenli bir şekilde geri getirir.

## 🔑 Temel Kavramlar

- **DbConnection** — veritabanına iletişim kanalını açar ve kapatır
- **DbCommand** — çalıştırılacak SQL talimatını taşır
- **DbParameter** — değerleri güvenli şekilde geçirir, SQL Injection'ı önler
- **DbDataReader** — sorgu sonuçlarını satır satır okur
- **DbTransaction** — işlemleri gruplar, ya hepsi başarılı olur ya hiçbiri
- **DbProviderFactory** — kodu veritabanı bağımsız tutar (SQL Server, SQLite, MySQL...)
