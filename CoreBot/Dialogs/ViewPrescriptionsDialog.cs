using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoreBot.Cards;
using CoreBot.DialogDetails;
using CoreBot.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace CoreBot.Dialogs;

public class ViewPrescriptionsDialog : ComponentDialog
{
    private readonly PrescriptionDataService _prescriptionDataService;

    private const string NameMessage = "What's the name on the prescriptions?";

    private const string NotFoundMessage = "I can't find any prescriptions under that name.";
    private const string OkMessage = "No problem! Here you go.";

    public ViewPrescriptionsDialog(PrescriptionDataService prescriptionDataService) : base(nameof(ViewPrescriptionsDialog))
    {
        _prescriptionDataService = prescriptionDataService;

        AddDialog(new TextPrompt(nameof(TextPrompt)));

        var waterfallSteps = new WaterfallStep[]
        {
            FirstNameStepAsync,
            NameActStepAsync,
        };

        AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
        InitialDialogId = nameof(WaterfallDialog);
    }

    private async Task<DialogTurnResult> FirstNameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        var details = (ViewPrescriptionsDetails)stepContext.Options;

        if (details.Name == null)
        {
            var promptMessage = MessageFactory.Text(NameMessage, NameMessage, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        return await stepContext.NextAsync(details.Name, cancellationToken);
    }

    private async Task<DialogTurnResult> NameActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        var name = (string)stepContext.Result;

        var prescriptions = await _prescriptionDataService.GetPrescriptionsByName(name);

        if (prescriptions.Any())
        {
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(OkMessage), cancellationToken);

            await stepContext.Context.SendActivityAsync(new Activity
            {
                Attachments = new List<Attachment> { PrescriptionsCard.From(prescriptions) },
                Type = ActivityTypes.Message
            }, cancellationToken);
        }
        else
        {
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(NotFoundMessage), cancellationToken);
        }

        return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
    }
}