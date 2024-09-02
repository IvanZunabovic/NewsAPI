using Domain.Primitives;
using Domain.ValueObjects;

namespace Domain.Entities;

public class News : Entity
{
    public string Title { get; set; }
    public Content Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid AuthorId { get; set; }
    virtual public Author Author { get; set; }

    public News(Guid id, string title, DateTime createdAt, Guid authorId)
        : base(id)
    {
        Title = title;
        CreatedAt = createdAt;
        AuthorId = authorId;

    }

    private News(Guid id, string title, Content content, DateTime createdAt, Guid authorId) : base(id)
    {
        Title = title;
        Content = content;
        CreatedAt = createdAt;
        AuthorId = authorId;
    }

    public static News Create(string title, Content content, DateTime createdAt, Guid authorId)
    {
        return new(Guid.NewGuid(), title, content, createdAt, authorId);
    }
}
