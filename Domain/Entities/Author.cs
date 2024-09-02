using Domain.Primitives;
using Domain.ValueObjects;

namespace Domain.Entities;

public class Author : Entity
{
    public FirstName FirstName { get; set; }
    public LastName LastName { get; set; }
    public string UserName { get; set; }
    public Email Email { get; set; }
    public string PasswordHash { get; set; }
    virtual public ICollection<News> News { get; set; }

    private Author(Guid id, string userName, string passwordHash) : base(id)
    {
        UserName = userName;
        PasswordHash = passwordHash;
    }

    public Author(Guid id, FirstName firstName, LastName lastName, string userName, Email email, string passwordHash) : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        UserName = userName;
        Email = email;
        PasswordHash = passwordHash;
    }

    public static Author Create(FirstName firstName, LastName lastName, string userName, Email email, string passwordHash)
    {
        return new(Guid.NewGuid(), firstName, lastName, userName, email, passwordHash);
    }
}
