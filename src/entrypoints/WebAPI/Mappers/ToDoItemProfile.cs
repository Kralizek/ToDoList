using AutoMapper;
using ToDoItemModel = WebAPI.ToDoItem;
using Item = Service.ToDoItem;
using System.Linq;
using Google.Protobuf.WellKnownTypes;
using Google.Protobuf.Collections;
using System;

namespace WebAPI.Mappers
{
    public class ToDoItemProfile : Profile
    {
        public ToDoItemProfile()
        {
            var map = CreateMap<Item, ToDoItemModel>();

            map.ForMember(o => o.DueDate, o => o.MapFrom(p => p.DueDate.ToDateTimeOffset()));

            map.ForMember(o => o.InsertedOn, o => o.MapFrom(p => p.InsertedOn.ToDateTimeOffset()));

            map.ForMember(o => o.Tags, o => o.MapFrom(p => p.Tags.ToArray()));

            map.ForMember(o => o.Id, o => o.MapFrom(p => Guid.Parse(p.Id)));

            var reverseMap = map.ReverseMap();

            reverseMap.ForMember(o => o.DueDate, o => o.MapFrom(p => Timestamp.FromDateTimeOffset(p.DueDate)));

            reverseMap.ForMember(o => o.InsertedOn, o => o.MapFrom(p => Timestamp.FromDateTimeOffset(p.InsertedOn)));

            reverseMap.ForMember(o => o.Id, o => o.MapFrom(p => p.Id.ToString()));

            reverseMap.ForMember(o => o.Tags, o => 
            {
                o.MapFrom((source, _) => 
                {
                    var field = new RepeatedField<string>();
                    
                    if (source.Tags != null)
                    { 
                        field.AddRange(source.Tags);
                    }
                    
                    return field;
                });

                o.NullSubstitute(new RepeatedField<string>());
            });
        }
    }
}