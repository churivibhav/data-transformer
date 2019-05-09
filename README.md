# Optimus
## A Data Transformer
Library to run data transformation jobs in a .NET Core application

## Introduction
With the help of this library, you can create an application that can run periodic or triggered jobs parallely.
It is originally designed to run a set of tasks to manipulate data in db tables, but can be extended to run any type of job.

A `Job` is a sequence of `Unit`s. Each unit performs a certain task. e.g. a unit of type `Sql` runs a DML script in the database.

There are two types of units already bundled --
1. Sql
1. Python

A `Sql` unit, as you guessed it, is used to run a DDL/DML to a database using ODBC connection.

To allow dynamic values to be used in these queries, each job can have it's `Variable`s. They can be referred inside units and the value is replaced at run time. e.g. In a `Sql` type unit, a variable named `x` is referred to as `${x}`. At the time of running the SQL, all variable names are replaced by their values.

A `Python` type unit can get and set variable values. It uses It also exposes functions such as `log(), sql(), query(), commit(), rollback()` to work with the database.

## Using the library

### With Filesystem and Sqlite (Reference Implementation)
### With AWS S3 and Snowflake (As AWS Lambda)
### With Custom Implementation

### Wiring up using Dependancy Injection

## Creating your own units
To create a new type of unit, implement `IJobUnit` interface.
