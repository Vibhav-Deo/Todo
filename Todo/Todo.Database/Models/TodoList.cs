namespace Todo.Database.Models;

using System;

public partial class TodoList
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; }
    public bool IsDeleted { get; set; }
    public string Description { get; set; }
    public virtual User User { get; set; }
}