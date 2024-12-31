using AdaptiveCards;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using Microsoft.Bot.Schema;

namespace CoreBot.Helpers;

public static class ChoicePromptHelper
{
    public static async Task<DialogTurnResult> PromptChoiceAsync(string text, List<string> options, WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        var actions = options.Select(choice => new AdaptiveSubmitAction
        {
            Title = choice,
            Data = choice
        }).ToList();

        return await PromptChoiceAsync(text, actions, stepContext, cancellationToken);
    }

    public static async Task<DialogTurnResult> PromptChoiceAsync(string text, List<AdaptiveSubmitAction> options, WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        var card = new AdaptiveCard(new AdaptiveSchemaVersion(1, 0))
        {
            Body = new List<AdaptiveElement>
            {
                new AdaptiveTextBlock
                {
                    Text = text,
                    Wrap = true
                }
            },
            Actions = options.ToList<AdaptiveAction>()
        };

        return await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions
        {
            Prompt = (Activity)MessageFactory.Attachment(new Attachment
            {
                ContentType = AdaptiveCard.ContentType,
                Content = JObject.FromObject(card)
            }),
            Choices = options.Select(o => new Choice(o.Data.ToString())).ToList(),
            Style = ListStyle.None
        }, cancellationToken);
    }
}