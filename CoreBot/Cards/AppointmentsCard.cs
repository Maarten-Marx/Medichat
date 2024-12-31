using System.Collections.Generic;
using System.Linq;
using AdaptiveCards;
using Data.Models;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;

namespace CoreBot.Cards;

public static class AppointmentsCard
{
    public static Attachment From(List<Appointment> appointments)
    {
        var card = new AdaptiveCard(new AdaptiveSchemaVersion(1, 0))
        {
            Body =
            [
                new AdaptiveTextBlock
                {
                    Text = "Your Appointments:",
                    Weight = AdaptiveTextWeight.Bolder,
                    Size = AdaptiveTextSize.Large
                },
                ..appointments.SelectMany(a => new List<AdaptiveElement>
                {
                    new AdaptiveFactSet
                    {
                        Facts =
                        [
                            new AdaptiveFact
                            {
                                Title = "Doctor:",
                                Value = a.Doctor?.Name ?? "Unknown"
                            },
                            new AdaptiveFact
                            {
                                Title = "Time:",
                                Value = a.Time
                            },
                            new AdaptiveFact
                            {
                                Title = "Reason:",
                                Value = a.Reason
                            }
                        ],
                        Separator = true,
                        Spacing = AdaptiveSpacing.Large
                    }
                })
            ]
        };

        var adaptiveCardAttachment = new Attachment
        {
            ContentType = "application/vnd.microsoft.card.adaptive",
            Content = JObject.FromObject(card)
        };

        return adaptiveCardAttachment;
    }
}