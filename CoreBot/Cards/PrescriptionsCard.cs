using System.Collections.Generic;
using System.Linq;
using AdaptiveCards;
using Data.Models;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;

namespace CoreBot.Cards;

public static class PrescriptionsCard
{
    public static Attachment From(List<Prescription> prescriptions)
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
                ..prescriptions.SelectMany(p => new List<AdaptiveElement>
                {
                    new AdaptiveFactSet
                    {
                        Facts =
                        [
                            new AdaptiveFact
                            {
                                Title = "Medicine:",
                                Value = p.Medicine?.Name ?? "Unknown"
                            },
                            new AdaptiveFact
                            {
                                Title = "Reason:",
                                Value = p.Reason
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