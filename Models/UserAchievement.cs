using System;
using System.Collections.Generic;

namespace BookCatalog.Models;

public partial class UserAchievement
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string AchievementCode { get; set; } = null!;

    public DateTime EarnedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
