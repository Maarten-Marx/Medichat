using System.Collections.Generic;
using System.Linq;
using AdaptiveCards;
using Data.Models;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;

namespace CoreBot.Cards;

public static class DoctorsCard
{
    public static Attachment From(List<Doctor> doctors)
    {
        var card = new AdaptiveCard(new AdaptiveSchemaVersion(1, 0))
        {
            Body =
            [
                new AdaptiveTextBlock
                {
                    Text = "Our Doctors",
                    Weight = AdaptiveTextWeight.Bolder,
                    Size = AdaptiveTextSize.Large
                },
                ..doctors.Select(d => new AdaptiveColumnSet
                    {
                        Columns =
                        [
                            new AdaptiveColumn
                            {
                                Width = AdaptiveColumnWidth.Auto,
                                Items =
                                [
                                    new AdaptiveImage
                                    {
                                        UrlString = d.Image,
                                        Size = AdaptiveImageSize.Small,
                                        Style = AdaptiveImageStyle.Person
                                    }
                                ]
                            },
                            new AdaptiveColumn
                            {
                                Width = AdaptiveColumnWidth.Stretch,
                                Items =
                                [
                                    new AdaptiveTextBlock
                                    {
                                        Text = d.Name,
                                        Weight = AdaptiveTextWeight.Bolder,
                                        Size = AdaptiveTextSize.Default
                                    },
                                    new AdaptiveTextBlock
                                    {
                                        Text = d.Title,
                                        Size = AdaptiveTextSize.Small
                                    },
                                ]
                            }
                        ]
                    }
                ).ToArray()
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