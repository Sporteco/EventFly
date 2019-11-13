using System;
using System.Collections.Generic;
using EventFly.Queries;
using Demo.Predicates;

namespace Demo.Queries
{
    public class EventPostersQuery : IQuery<EventPosters>
    {
        public StringPredicate Title { get; set; }
        public StringPredicate EventTypeCode { get; set; }
        public StringPredicate OrganizerName { get; set; }
        public EventPlacePredicate Place { get; set; }
        public StringPredicate SightName { get; set; }
        public StringPredicate EntertainmentName { get; set; }
        public StringPredicate HospitalityServiceName { get; set; }
        public StringPredicate CelebrityName { get; set; }
        public CompetitionPostersPredicate Competitions { get; set; }
        public DateTimeRangePredicate Period { get; set; }
    }


    public class EventPosters
    {
        public IEnumerable<EventPoster> Items { get; set; }
    }

    public class EventPoster
    {
        public EventPosterId Id { get; set; }
        public PosterEventType EventType { get; set; }
        public string Title { get; set; }
        public EventOrganizer Organizer { get; set; }
        public EventPlace Place { get; set; }
        public EventPeriod Period { get; set; }
        public string Idea { get; set; }
        public EventSights Sights { get; set; }
        public EventEntertainments Entertainments { get; set; }
        public EventHospitalityServices HospitalityServices { get; set; }

        public EventCelebrities Celebrities { get; set; }

        //public Identity MediaAlbumId { get; set; }
        public IEnumerable<EventPartner> Partners { get; set; }
        public IEnumerable<EventLink> Links { get; set; }
        public IEnumerable<EventDescription> Descriptions { get; set; }
        public IEnumerable<CompetitionPoster> Competitions { get; set; }
        public Uri MainImage { get; set; }
        public Uri BackgroundImage { get; set; }
    }

    public class PosterEventType
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }

    public class EventDescription
    {
        public string Title { get; set; }
        public string Text { get; set; }
    }

    public class EventLink
    {
        public string Title { get; set; }
        public Uri Uri { get; set; }
    }

    public class EventPartner
    {
        public Uri Logo { get; set; }
        public string Name { get; set; }
        public Uri Link { get; set; }
        public string Description { get; set; }
    }

    public class EventCelebrities
    {
        public string Description { get; set; }
        public IEnumerable<EventCelebrity> Items { get; set; }
    }

    public class EventCelebrity
    {
        public string Name { get; set; }
        public string Awards { get; set; }
        public Uri Photo { get; set; }
    }

    public class EventHospitalityServices
    {
        public string Description { get; set; }
        public IEnumerable<EventHospitalityService> Items { get; set; }
    }

    public class EventHospitalityService
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class EventEntertainments
    {
        public string Description { get; set; }
        public IEnumerable<EventEntertainment> Items { get; set; }
    }

    public class EventEntertainment
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class EventSights
    {
        public string Description { get; set; }
        public IEnumerable<EventSight> Items { get; set; }
    }

    public class EventSight
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class EventPeriod
    {
        public DateTime Begin { get; set; }
        public DateTime End { get; set; }
    }

    public class Address
    {
        public EventPosterCountry Country { get; set; }
        public EventPosterRegion Region { get; set; }
        public EventPosterSettlement Settlement { get; set; }
        public string AddressInsideSettlement { get; set; }
    }

    public class EventPosterCountry
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }

    public class EventPosterRegion
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }

    public class EventPosterSettlement
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }

    public class Location
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class EventPlace
    {
        public Address Address { get; set; }
        public Location Location { get; set; }
        public string HowToGetTo { get; set; }
        public string Description { get; set; }
    }

    public class EventOrganizer
    {
        public string Name { get; set; }
        public IEnumerable<Contact> Contacts { get; set; }
    }

    public class Contact
    {
        public string Value { get; set; }
        public string Title { get; set; }
    }

    public class CompetitionPoster
    {
        public CompetitionDiscipline Discipline { get; set; }
        public SlotCost SlotCost { get; set; }
        public IEnumerable<CompetitionPrize> Prizes { get; set; }
        public IEnumerable<CompetitionStarterPackItem> StarterPacks { get; set; }
        public string Description { get; set; }
    }

    public class CompetitionDiscipline
    {
        public string Name { get; set; }
        public Uri Icon { get; set; }
        public string Code { get; set; }
    }

    public class CompetitionStarterPackItem
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Uri Icon { get; set; }
    }

    public class CompetitionPrize
    {
        public string Name { get; set; }
        public Money MonetaryEquivalent { get; set; }
        public string Description { get; set; }
    }

    public class SlotCost
    {
        public Money Minimum { get; set; }
        public Money Maximum { get; set; }
    }

    public class Money
    {
        public decimal Amount { get; set; }
    }

    public sealed class EventPlacePredicate : ObjectPredicate<EventPlace>
    {
        public AddressPredicate Address { get; set; }
        public LocationPredicate Location { get; set; }

        public override bool Check(EventPlace @object)
        {
            return (Address?.Check(@object.Address) ?? true)
                && (Location?.Check(new LocationPoint { Latitude = @object.Location.Latitude, Longitude = @object.Location.Longitude }) ?? true);
        }
    }

    public sealed class DisciplinePredicate : ObjectPredicate<CompetitionDiscipline>
    {
        public StringPredicate Code { get; set; }
        public override bool Check(CompetitionDiscipline @object)
        {
            return Code?.Check(@object.Code) ?? true;
        }
    }

    public sealed class CompetitionPrizePredicate : ObjectPredicate<CompetitionPrize>
    {

        public StringPredicate Name { get; set; }

        public override bool Check(CompetitionPrize @object)
        {
            return Name?.Check(@object.Name) ?? true;
        }
    }

    public sealed class CompetitionPrizesPredicate : CollectionPredicate<CompetitionPrize, CompetitionPrizePredicate> { }

    public sealed class CompetitionStarterPackItemPredicate : ObjectPredicate<CompetitionStarterPackItem>
    {
        public StringPredicate Name { get; set; }

        public override bool Check(CompetitionStarterPackItem @object)
        {
            return Name?.Check(@object.Name) ?? true;
        }
    }

    public sealed class CompetitionStarterPackItemsPredicate : CollectionPredicate<CompetitionStarterPackItem, CompetitionStarterPackItemPredicate> { }

    public sealed class CompetitionPosterPredicate : ObjectPredicate<CompetitionPoster>
    {
        public DisciplinePredicate Discipline { get; set; }
        public CompetitionPrizesPredicate Prizes { get; set; }
        public CompetitionStarterPackItemsPredicate StarterPackItems { get; set; }
        public DecimalRangePredicate SlotCost { get; set; }
        public StringPredicate Description { get; set; }

        public override bool Check(CompetitionPoster @object)
        {
            return (Discipline?.Check(@object.Discipline) ?? true)
                && (Prizes?.Check(@object.Prizes) ?? true)
                && (StarterPackItems?.Check(@object.StarterPacks) ?? true)
                && (Description?.Check(@object.Description) ?? true)
                && ((SlotCost?.Check(@object.SlotCost.Minimum.Amount) ?? true) 
                    || ((bool) SlotCost?.Check(@object.SlotCost.Maximum.Amount)));
        }
    }

    public sealed class CompetitionPostersPredicate : CollectionPredicate<CompetitionPoster, CompetitionPosterPredicate> { }

    public sealed class AddressPredicate : ObjectPredicate<Address>
    {
        public StringPredicate CountryCode { get; set; }
        public StringPredicate RegionCode { get; set; }
        public StringPredicate SettlementCode { get; set; }

        public override bool Check(Address @object)
        {
            return (CountryCode != null && RegionCode != null && SettlementCode != null && SettlementCode.Check(@object.Settlement.Code))
                   || (CountryCode != null && RegionCode != null && SettlementCode == null && RegionCode.Check(@object.Region.Code))
                   || (CountryCode != null && RegionCode == null && SettlementCode == null && CountryCode.Check(@object.Country.Code))
                   || (CountryCode == null && RegionCode == null && SettlementCode == null);
        }
    }

}
