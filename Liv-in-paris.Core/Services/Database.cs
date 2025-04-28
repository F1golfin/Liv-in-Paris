using Liv_in_paris.Core.Models;

namespace Liv_in_paris.Core.Services;

public static class Database
{
    private static readonly DatabaseManager _instance = new DatabaseManager("localhost", "livin_paris", "root", "root");

    public static DatabaseManager Instance => _instance;
}