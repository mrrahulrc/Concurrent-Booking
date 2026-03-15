Create Database IF NOT EXISTS "Movie_Management";

Create Table IF NOT EXISTS Seats(
  id          SERIAL PRIMARY KEY,
  username    varchar(255) default null
);

insert into Seats (username) values (null);