using Edvantix.SharedKernel.SeedWork;
using Edvantix.System.Domain.AggregatesModel.LimitAggregate;

namespace Edvantix.System.Domain.AggregatesModel.SubscriptionAggregate;

public sealed class Subscription() : Entity<long>, IAggregateRoot
{
    private readonly List<Limit> _limits = new();
    
    public Subscription(
        string name,
        SubscriptionType type,
        decimal price,
        long currencyId) : this()
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Название подписки не должно быть пустым.", nameof(name));
        
        if (price < 0)
            throw new ArgumentException("Цена не может быть отрицательной.", nameof(price));
        
        if (currencyId <= 0)
            throw new ArgumentException("Проблема с идентификатором валюты", nameof(currencyId));
        
        Name = name;
        Type = type;
        Price = price;
        CurrencyId = currencyId;
    }
    
    public string Name { get; set; } = null!;
    public SubscriptionType Type { get; set; }
    public decimal Price { get; set; }
    public long CurrencyId { get; set; }

    public IReadOnlyCollection<Limit> Limits => _limits.AsReadOnly();
    
    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Название подписки не должно быть пустым.", nameof(newName));

        Name = newName;
    }

    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice < 0)
            throw new ArgumentException("Цена не может быть отрицательной.", nameof(newPrice));

        Price = newPrice;
    }

    public void ChangeType(SubscriptionType newType)
    {
        Type = newType;
    }

    public void ChangeCurrency(long newCurrencyId)
    {
        if (newCurrencyId <= 0)
            throw new ArgumentException("Проблема с идентификатором валюты", nameof(newCurrencyId));

        CurrencyId = newCurrencyId;
    }

    // Методы для управления лимитами
    public void AddLimit(LimitType type, decimal value)
    {
        if (_limits.Any(l => l.Type == type))
            throw new InvalidOperationException($"Ограничение вида {type} уже существует для данного вида подписки.");

        var limit = new Limit(Id, type, value);
        _limits.Add(limit);
    }

    public void RemoveLimit(LimitType type)
    {
        var limit = _limits.FirstOrDefault(l => l.Type == type);
        if (limit == null)
            throw new InvalidOperationException($"Ограничение вида {type} не существует для данного вида подписки.");

        _limits.Remove(limit);
    }

    public void UpdateLimit(LimitType type, decimal newValue)
    {
        var limit = _limits.FirstOrDefault(l => l.Type == type);
        if (limit == null)
            throw new InvalidOperationException($"Ограничение вида {type} не существует для данного вида подписки.");

        limit.UpdateValue(newValue);
    }

    public Limit? GetLimit(LimitType type)
    {
        return _limits.FirstOrDefault(l => l.Type == type);
    }
}
