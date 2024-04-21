using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Entities;

public class SubscribersEntity
{
    [Key]
    public string Email { get; set; } = null!;
    public bool DailyNewsLetter { get; set; }
    public bool AdvertisingUpdates { get; set; }
    public bool WeekInReview { get; set; }
    public bool EventUpdates { get; set; }
    public bool StartupsWeekly { get; set; }
    public bool Podcasts { get; set; }
}
