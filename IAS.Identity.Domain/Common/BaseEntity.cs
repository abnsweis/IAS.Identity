namespace IAS.Identity.Domain.Common.Constants;

public abstract class BaseEntity
{
    public Guid Id { get; set; }

    public bool IsDeleted { get; private set; }

    public void Delete()
    {
        IsDeleted = true;
    }
}