using System.Collections.Generic;
using NationsAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using NationsAPI.Repositories;

namespace NationsAPI.Database
{
    public static class Seeder
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Player>().HasData(PreparePlayers());
        }

        private static IList<Player> PreparePlayers()
        {
            return new List<Player> {
                new Player{Id=1, Nick="nations", Password="YsiwFeSpgAA0qnBxQVqfJxsUxSPLHBKinJuFr89QJ8iPIBHk"}, //pass: a
            };
        }
    }
}