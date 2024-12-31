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

public class ViewAppointmentsDialog : ComponentDialog
{
    private readonly AppointmentDataService _appointmentDataService;

    private const string NameMessage = "What's the name on the appointments?";

    private const string NotFoundMessage = "I can't find any appointments under that name.";
    private const string OkMessage = "No problem! Here you go.";

    public ViewAppointmentsDialog(AppointmentDataService appointmentDataService) : base(nameof(ViewAppointmentsDialog))
    {
        _appointmentDataService = appointmentDataService;

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
        var details = (ViewAppointmentsDetails)stepContext.Options;

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

        var appointments = await _appointmentDataService.GetAppointmentsByName(name);

        if (appointments.Any())
        {
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(OkMessage), cancellationToken);

            await stepContext.Context.SendActivityAsync(new Activity
            {
                Attachments = new List<Attachment> { AppointmentsCard.From(appointments) },
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