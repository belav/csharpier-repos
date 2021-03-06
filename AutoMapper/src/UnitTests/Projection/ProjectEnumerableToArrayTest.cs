using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Shouldly;

namespace AutoMapper.UnitTests.Projection
{
    using QueryableExtensions;

    public class ProjectEnumerableToArrayTest
    {
        private MapperConfiguration _config;

        public ProjectEnumerableToArrayTest()
        {
            _config = new MapperConfiguration(cfg =>
            {
                cfg.CreateProjection<Movie, MovieDto>();
                cfg.CreateProjection<Actor, ActorDto>();
            });
        }

        [Fact]
        public void EnumerablesAreMappedToArrays()
        {
            var movies = 
                new List<Movie>() {
                new Movie() { Actors = new Actor[] { new Actor() { Name = "Actor 1" }, new Actor() { Name = "Actor 2" } } },
                new Movie() { Actors = new Actor[] { new Actor() { Name = "Actor 3" }, new Actor() { Name = "Actor 4" } } }
                }.AsQueryable();

            var mapped = movies.ProjectTo<MovieDto>(_config);

            mapped.ElementAt(0).Actors.Length.ShouldBe(2);
            mapped.ElementAt(1).Actors[1].Name.ShouldBe("Actor 4");
        }

        public class Movie
        {
            public IEnumerable<Actor> Actors { get; set; }
        }

        public class MovieDto
        {
            public ActorDto[] Actors { get; set; }
        }

        public class Actor
        {
            public string Name { get; set; }
        }

        public class ActorDto
        {
            public string Name { get; set; }
        }
    }
}
