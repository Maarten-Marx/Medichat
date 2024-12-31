using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CoreBot.Cards;
using CoreBot.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace CoreBot.Dialogs;

public class ViewDoctorsDialog : ComponentDialog
{
    private readonly DoctorDataService _doctorDataService;

    private const string OkMessage = "No problem! Here you go.";

    public ViewDoctorsDialog(DoctorDataService doctorDataService) : base(nameof(ViewDoctorsDialog))
    {
        _doctorDataService = doctorDataService;

        AddDialog(new TextPrompt(nameof(TextPrompt)));
        AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));

        var waterfallSteps = new WaterfallStep[]
        {
            FirstActStepAsync,
        };

        AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
        InitialDialogId = nameof(WaterfallDialog);
    }

    private async Task<DialogTurnResult> FirstActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        var doctors = await _doctorDataService.GetDoctorsAsync();

        await stepContext.Context.SendActivityAsync(MessageFactory.Text(OkMessage), cancellationToken);

        await stepContext.Context.SendActivityAsync(new Activity
        {
            Attachments = new List<Attachment> { DoctorsCard.From(doctors) },
            Type = ActivityTypes.Message
        }, cancellationToken);

        return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
    }
}