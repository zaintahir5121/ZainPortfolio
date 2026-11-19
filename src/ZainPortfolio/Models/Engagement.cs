using System.ComponentModel.DataAnnotations;

namespace ZainPortfolio.Models;

/// <summary>Newsletter / blog subscription.</summary>
public class Subscriber
{
    public int Id { get; set; }
    [Required, MaxLength(200), EmailAddress] public string Email { get; set; } = "";
    [MaxLength(120)] public string? Name { get; set; }
    [MaxLength(60)] public string Source { get; set; } = "site";     // site | blog | footer | popup
    public bool IsConfirmed { get; set; }
    public bool IsActive { get; set; } = true;
    [MaxLength(64)] public string? ConfirmToken { get; set; }
    [MaxLength(64)] public string? IpHash { get; set; }
    public DateTime SubscribedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ConfirmedAt { get; set; }
    public DateTime? UnsubscribedAt { get; set; }
}

/// <summary>
/// One row per non-bot page request. Kept deliberately small — this powers the
/// admin analytics dashboard without needing an external analytics product.
/// </summary>
public class PageView
{
    public long Id { get; set; }
    [Required, MaxLength(400)] public string Path { get; set; } = "";
    [MaxLength(30)] public string PageType { get; set; } = "other";   // home | project | blog | list | other
    public int? EntityId { get; set; }
    [MaxLength(300)] public string? Title { get; set; }
    [MaxLength(400)] public string? Referrer { get; set; }
    [MaxLength(60)] public string? ReferrerHost { get; set; }
    [MaxLength(300)] public string? UserAgent { get; set; }
    [MaxLength(30)] public string? DeviceType { get; set; }           // desktop | mobile | tablet
    [MaxLength(64)] public string? VisitorId { get; set; }
    [MaxLength(64)] public string? IpHash { get; set; }
    [MaxLength(8)] public string? CountryCode { get; set; }
    public bool IsNewVisitor { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateOnly Day { get; set; }
}
