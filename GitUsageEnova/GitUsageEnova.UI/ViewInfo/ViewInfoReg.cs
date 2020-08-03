using Soneta.Business.UI;

[assembly: FolderView("GIT commits",
    Priority = 0,
    Description = "Test",
    BrickColor = FolderViewAttribute.BlueBrick,
    Icon = "TableFolder.ico"
)]

[assembly: FolderView("GIT commits/Ilość commit-ów",
    Priority = 0,
    Description = "Ilość commit-ów, które dana osoba wprowadziła danego dnia",
    TableName = "CommitsPerDay",
    ViewType = typeof(GitUsageEnova.UI.CommitsPerDayViewInfo)
)]

[assembly: FolderView("GIT commits/Średnia ilość commitów",
    Priority = 100,
    Description = "Średnia ilość commit-ów, które dana osoba wprowadziła danego dnia",
    TableName = "CommitsPerDay",
    ViewType = typeof(GitUsageEnova.UI.AveragesPerDayViewInfo)
)]