using System;
using System.Threading;
using System.Threading.Tasks;
using CoreBot.CognitiveModels;
using CoreBot.DialogDetails;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;

namespace CoreBot.Dialogs;

public class MainDialog : ComponentDialog
{
    private readonly ILogger _logger;
    private MedichatCluRecognizer _recongizer;

    private const string InitialMessage = "What would you like to do?";
    private const string FollowupMessage = "What else can I do for you?";
    private const string NotConfiguredMessage = "ERROR: CLU not configured";

    public MainDialog(
        MedichatCluRecognizer medichatCluRecognizer,
        PlaceAppointmentDialog placeAppointmentDialog,
        ViewAppointmentsDialog viewAppointmentsDialog,
        ViewDoctorsDialog viewDoctorsDialog,
        RequestPrescriptionDialog requestPrescriptionDialog,
        ViewPrescriptionsDialog viewPrescriptionsDialog,
        ILogger<MainDialog> logger
    ) : base(nameof(MainDialog))
    {
        _logger = logger;
        _recongizer = medichatCluRecognizer;

        AddDialog(new TextPrompt(nameof(TextPrompt)));

        AddDialog(placeAppointmentDialog);
        AddDialog(viewAppointmentsDialog);
        AddDialog(viewDoctorsDialog);
        AddDialog(requestPrescriptionDialog);
        AddDialog(viewPrescriptionsDialog);

        var waterfallSteps = new WaterfallStep[]
        {
            FirstActionStepAsync,
            ActionActStepAsync,
            FinalStepAsync,
        };
        AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));

        InitialDialogId = nameof(WaterfallDialog);
    }

    private async Task<DialogTurnResult> FirstActionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        if (!_recongizer.IsConfigured)
        {
            throw new InvalidOperationException(NotConfiguredMessage);
        }

        var messageText = stepContext.Options?.ToString() ?? InitialMessage;
        return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput) }, cancellationToken);
    }

    private async Task<DialogTurnResult> ActionActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        var result = await _recongizer.RecognizeAsync<MedichatModel>(stepContext.Context, cancellationToken);

        object details;

        switch (result.GetTopIntent().intent)
        {
            case MedichatModel.Intent.PlaceAppointment:
                details = new PlaceAppointmentDetails
                {
                    Name = result.Entities.GetName(),
                    Contact = result.Entities.GetContact(),
                    Reason = result.Entities.GetReason(),
                    Time = result.Entities.GetTime(),
                };
                return await stepContext.BeginDialogAsync(nameof(PlaceAppointmentDialog), details, cancellationToken);
            case MedichatModel.Intent.ViewDoctors:
                return await stepContext.BeginDialogAsync(nameof(ViewDoctorsDialog), cancellationToken: cancellationToken);
            case MedichatModel.Intent.ViewAppointments:
                details = new ViewAppointmentsDetails
                {
                    Name = result.Entities.GetName(),
                };
                return await stepContext.BeginDialogAsync(nameof(ViewAppointmentsDialog), details, cancellationToken);
            case MedichatModel.Intent.RequestPrescription:
                details = new RequestPrescriptionDetails
                {
                    Name = result.Entities.GetName(),
                    Reason = result.Entities.GetReason(),
                };
                return await stepContext.BeginDialogAsync(nameof(RequestPrescriptionDialog), details, cancellationToken);
            case MedichatModel.Intent.ViewPrescriptions:
                details = new ViewPrescriptionsDetails
                {
                    Name = result.Entities.GetName(),
                };
                return await stepContext.BeginDialogAsync(nameof(ViewPrescriptionsDialog), details, cancellationToken);
            default:
                return await stepContext.NextAsync(null, cancellationToken);
        }
    }

    private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        return await stepContext.ReplaceDialogAsync(InitialDialogId, FollowupMessage, cancellationToken);
    }
}