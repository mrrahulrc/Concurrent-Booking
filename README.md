# Concurrent Seat Booking Simulation

A .NET console application that demonstrates how to safely handle **concurrent booking requests** using **PostgreSQL row-level locking (`SELECT ... FOR UPDATE`)**.

This project simulates multiple users trying to **book the same seat simultaneously**, similar to real-world systems such as movie ticket booking, airline reservations, hotel bookings, and limited inventory e-commerce.

The goal is to demonstrate how **database transactions and locking prevent double bookings** when multiple requests hit the system at the same time.

---

# Problem

In real-world booking systems, many users may attempt to reserve the **same seat or resource simultaneously**.

If concurrency is not handled properly, it can result in:

- Double bookings
- Data inconsistency
- Incorrect confirmations to users
- Inventory mismatch

Handling **race conditions** is a critical part of backend system design.

---

# Solution Demonstrated

This project uses **PostgreSQL transactions with row-level locking**.

When a user attempts to book a seat:

1. A transaction begins
2. The seat row is locked using `SELECT ... FOR UPDATE`
3. Only one transaction can update the row
4. All other transactions fail once the seat is already booked

This guarantees **data consistency under concurrent requests**.

---

# Tech Stack

- .NET (C# Console Application)
- PostgreSQL
- Dapper (Micro ORM)
- Async / Parallel Programming

---

# System Architecture

```
+-----------------------+
|   .NET Console App    |
|  (Concurrent Tasks)   |
+----------+------------+
           |
           | Multiple users try to book
           | the same seat concurrently
           v
+-----------------------+
|  BookingStimulation   |
|  (Transaction Logic)  |
+----------+------------+
           |
           | SELECT ... FOR UPDATE
           v
+-----------------------+
|     PostgreSQL DB     |
|   Row-Level Locking   |
+-----------------------+
```

The database ensures **only one transaction updates the seat record**.

---

# Concurrency Flow

```
Users: Sheldon, Jack, Mahito, Drack, Neza
             |
             v
    Parallel Booking Requests
             |
             v
      Start Transactions
             |
             v
SELECT seat WHERE username IS NULL FOR UPDATE
             |
     +-------+--------+
     |                |
Transaction 1     Transaction 2+
 Locks the row      Waits / Fails
     |
     v
Seat gets booked
     |
     v
Other transactions fail
```

---

# Example Console Output

```
---------------------Seats reset successfully.---------------------

Sheldon, Jack, Mahito, Drack, Neza are Trying to book the same seat numbered 1.

Seat booked successfully for Jack
Failed to book seat for Mahito
Failed to book seat for Sheldon
Failed to book seat for Drack
Failed to book seat for Neza
```

Each run may produce **a different successful user**, but **only one booking will succeed**.

---

# Project Structure

```
Concurrent_Booking
│
├── Program.cs
├── BookingStimulation.cs
├── DbHelper.cs
└── Seats.cs
```

---

# Core Components

## Program.cs

Entry point that starts the booking simulation.

```csharp
BookingStimulation bookingStimulation = new BookingStimulation();
await bookingStimulation.Start();
```

---

## BookingStimulation.cs

Responsible for:

- Resetting seats
- Simulating concurrent users
- Handling booking transactions

Core query:

```sql
SELECT id, username
FROM seats
WHERE id = 1 AND username IS NULL
FOR UPDATE;
```

This locks the seat row so **other transactions cannot modify it simultaneously**.

---

## DbHelper.cs

Handles database connection creation using **Npgsql**.

⚠️ **Important:**  
Update the database configuration if your PostgreSQL setup is different.

```csharp
builder.Host = "localhost";
builder.Port = 5432;
builder.Username = "postgres";
builder.Password = "mysecretpassword";
builder.Database = "Movie_Management";
```

Make sure these values match your **local PostgreSQL configuration** before running the project.

---

## Seats.cs

Represents the seat model.

```csharp
public class Seats
{
    public int id { get; set; }
    public string? username { get; set; }
}
```

---

# Database Setup

Before running the application, execute the **initial database setup script**.

Create the table:

```sql
CREATE TABLE seats (
    id SERIAL PRIMARY KEY,
    username TEXT NULL
);
```

Insert the initial seat record:

```sql
INSERT INTO seats (id, username)
VALUES (1, NULL);
```

Alternatively, if the repository contains an **init SQL script**, run that script to initialize the database.

---

# Running the Application

Clone the repository:

```
git clone https://github.com/yourusername/concurrent-seat-booking.git
cd concurrent-seat-booking
```

Run the project:

```
dotnet run
```

Press **1 + Enter** to run the simulation again.

---

# Key Concepts Demonstrated

- Concurrent programming in .NET
- Database transactions
- Row-level locking
- Preventing race conditions
- `SELECT ... FOR UPDATE`
- Parallel execution using `Task`

---

# Real World Applications

This pattern is commonly used in systems like:

- Movie ticket booking platforms
- Airline seat reservations
- Hotel booking platforms
- Flash sale inventory systems
- Event ticketing systems

---

# Learning Outcome

This project demonstrates how **database-level locking mechanisms protect data integrity in highly concurrent environments**.

Understanding these patterns is essential when designing **scalable booking, reservation, and inventory systems**.