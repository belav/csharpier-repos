namespace AutoMapper.UnitTests;

public class When_using_a_member_name_replacer : NonValidatingSpecBase
{
    public class Source
    {
        public int Value { get; set; }
        public int �v�ator { get; set; }
        public int SubAirlinaFlight { get; set; }
    }

    public class Destination
    {
        public int Value { get; set; }
        public int Aviator { get; set; }
        public int SubAirlineFlight { get; set; }
    }

    [Fact]
    public void Should_map_properties_with_different_names()
    {
        var config = new MapperConfiguration(c =>
        {
            c.ReplaceMemberName("�", "A");
            c.ReplaceMemberName("�", "i");
            c.ReplaceMemberName("Airlina", "Airline");
            c.CreateMap<Source, Destination>();
        });

        var source = new Source()
        {
            Value = 5,
            �v�ator = 3,
            SubAirlinaFlight = 4
        };

        //Mapper.AddMemberConvention().AddName<ReplaceName>(_ => _.AddReplace("A", "�").AddReplace("i", "�").AddReplace("Airline", "Airlina")).SetMemberInfo<FieldPropertyMemberInfo>();
        var mapper = config.CreateMapper();
        var destination = mapper.Map<Source, Destination>(source);

        Assert.Equal(source.Value, destination.Value);
        Assert.Equal(source.�v�ator, destination.Aviator);
        Assert.Equal(source.SubAirlinaFlight, destination.SubAirlineFlight);
    }
}

