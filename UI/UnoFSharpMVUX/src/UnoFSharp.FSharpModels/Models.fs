namespace UnoFSharp.FSharpModels

open System

/// Simple F# record for testing
type Person = {
    FirstName: string
    LastName: string
    Age: int
}

/// F# record with optional fields
type Product = {
    Id: Guid
    Name: string
    Price: decimal
    Description: string option
}

/// Nested F# record
type Address = {
    Street: string
    City: string
    ZipCode: string
}

type Customer = {
    Id: int
    Name: string
    Email: string
    Address: Address
}

/// Helper module with factory functions
module PersonModule =
    let create firstName lastName age = {
        FirstName = firstName
        LastName = lastName
        Age = age
    }
    
    let fullName person = 
        sprintf "%s %s" person.FirstName person.LastName

module ProductModule =
    let create name price = {
        Id = Guid.NewGuid()
        Name = name
        Price = price
        Description = None
    }
    
    let withDescription desc product =
        { product with Description = Some desc }

/// Todo item for interactive testing
type TodoItem = {
    Id: Guid
    Title: string
    Description: string
    IsCompleted: bool
    CreatedAt: DateTime
}

type TodoPriority =
    | Low
    | Medium
    | High

type TodoTask = {
    Id: Guid
    Title: string
    Priority: TodoPriority
    DueDate: DateTime option
    IsCompleted: bool
}

module TodoModule =
    let createItem title description = {
        Id = Guid.NewGuid()
        Title = title
        Description = description
        IsCompleted = false
        CreatedAt = DateTime.Now
    }
    
    let complete item =
        { item with IsCompleted = true }
    
    let uncomplete item =
        { item with IsCompleted = false }
    
    let toggle item =
        { item with IsCompleted = not item.IsCompleted }
    
    let updateTitle newTitle item =
        { item with Title = newTitle }
    
    let createTask title priority dueDate = {
        Id = Guid.NewGuid()
        Title = title
        Priority = priority
        DueDate = dueDate
        IsCompleted = false
    }
    
    let priorityToString = function
        | Low -> "Low"
        | Medium -> "Medium"
        | High -> "High"
