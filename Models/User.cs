using System;
using System.Collections.Generic;

namespace BookCatalog.Models;

public partial class User
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string? PasswordHash { get; set; }

    public string DisplayName { get; set; } = null!;

    public string? AvatarPath { get; set; }

    public string? GoogleId { get; set; }

    public string SubscriptionPlan { get; set; } = null!;

    public int DailyGoalPages { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<UserAchievement> UserAchievements { get; set; } = new List<UserAchievement>();

    public virtual ICollection<UserBook> UserBooks { get; set; } = new List<UserBook>();
}
