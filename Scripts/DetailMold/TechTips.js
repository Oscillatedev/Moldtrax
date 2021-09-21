$(document).ready(function ()
{

$('.textarea-editor').summernote(
    {
        toolbar: [
            ['style', ['style']],
            ['fontsize', ['fontsize']],
            ['font', ['bold', 'italic', 'underline', 'clear']],
            ['fontname', ['fontname']],
            ['color', ['color']],
            ['para', ['ul', 'ol', 'paragraph']],
            ['height', ['height']],
            ['insert', ['link', 'picture', 'hr']],
            ['view', ['fullscreen', 'codeview', 'help']]
        ],
        height: 550,                 // set editor height
        minHeight: null,
        popover: {
            link: []
        },// set minimum height of editor
        maxHeight: null,
        focus: true
        // set maximum height of editor
    });
$('.note-editable').css('font-size', '12px');



$(document).ready(function () {
    $('.note-editable').trigger('focus');
})

function GetFocusOnEditor() {
    $('.note-editable').trigger('focus');
}

$(".note-toolbar").click(function () {
    debugger
    var data = $(this);
    data.find(".note-editable").trigger('focus');
});

$("#MCWeight").focusout(function () {
    var Weight = $("#MCWeight").val().replace(/\,/g, '');
    $("#MCWeight").val(addCommas(Weight));
})


    $("#SavetechTips11").click(function () {
    debugger;
    //var data = $("#TechTipSbmtForm").serialize();
    var obj = new Object();
    obj.MCHeight = $("#MCHeight").val();
    obj.MCWidth = $("#MCWidth").val();
    obj.MCDepth = $("#MCDepth").val();
    obj.MCWeight = $("#MCWeight").val();
    obj.MCWidthOpen = $("#MCWidthOpen").val();
    obj.MCEjectorStroke = $("#MCEjectorStroke").val();
    obj.MCTotalHeight = $("#MCTotalHeight").val();
    obj.TTHRSystem = $("#TTHRSystem").val();
    obj.TTHRSerialNumber = $("#TTHRSerialNumber").val();
    obj.TTHRProgramNumber = $("#TTHRProgramNumber").val();
    obj.TTHRType = $("#TTHRType").val();
    obj.TTHRActuation = $("#TTHRActuation").val();
    obj.TTHRProbeType = $("#TTHRProbeType").val();
    obj.TTHRController = $("#TTHRController").val();
    obj.TTHRNumberZones = $("#TTHRNumberZones").val();
    obj.TTHRNumberDrops = $("#TTHRNumberDrops").val();
    obj.TTHROpenPressureMax = $("#TTHROpenPressureMax").val();
    obj.TTHROpenPressureTypical = $("#TTHROpenPressureTypical").val();
    obj.TTHRClosePressureMax = $("#TTHRClosePressureMax").val();
    obj.TTHRClosePressureTypical = $("#TTHRClosePressureTypical").val();
    obj.TTHRProbeHeater = $("#TTHRProbeHeater").val();
    obj.TTHRProbeHeaterThermoType = $("#TTHRProbeHeaterThermoType").val();
    obj.TTHRManifoldHeater = $("#TTHRManifoldHeater").val();
    obj.TTHRManifoldHeaterThermoType = $("#TTHRManifoldHeaterThermoType").val();
    obj.BridgeHeater = $("#BridgeHeater").val();
    obj.BridgeThermocouple = $("#BridgeThermocouple").val();
    obj.SprueHeater = $("#SprueHeater").val();
    obj.SprueThermocouple = $("#SprueThermocouple").val();
    obj.TTHRMaxOperatTemp = $("#TTHRMaxOperatTemp").val();
    obj.TTHRClampPlateBoltTorque = $("#TTHRClampPlateBoltTorque").val();
    obj.TechTipsID = $("#TechTipsID").val();
    obj.MoldDataID = $("#MoldDataID").val();


    if (document.getElementById('YesRadio').checked) {
        obj.TTHRDropsServicableInPress = true;
    }
    else {
        obj.TTHRDropsServicableInPress = false;
    }

    var TTHRDisassembly = $("#Editor0").html();
    var TTHRDisassembly1 = $.parseHTML(TTHRDisassembly);
    obj.TTHRDisassembly = TTHRDisassembly1[1].innerHTML;


    var TTHRClean = $("#Editor1").html();
    var TTHRClean1 = $.parseHTML(TTHRClean);
    obj.TTHRClean = TTHRClean1[1].innerHTML;

    var TTHRAssembly = $("#Editor2").html();
    var TTHRAssembly1 = $.parseHTML(TTHRAssembly);
    obj.TTHRAssembly = TTHRAssembly1[1].innerHTML;

    var TTHRFinalChk = $("#Editor3").html();
    var TTHRFinalChk1 = $.parseHTML(TTHRFinalChk);
    obj.TTHRFinalChk = TTHRFinalChk1[1].innerHTML;

    var TTHRPolishing = $("#Editor4").html();
    var TTHRPolishing1 = $.parseHTML(TTHRPolishing);
    obj.TTHRPolishing = TTHRPolishing1[1].innerHTML;

    var TTHRToolKit = $("#Editor5").html();
    var TTHRToolKit1 = $.parseHTML(TTHRToolKit);
    obj.TTHRToolKit = TTHRToolKit1[1].innerHTML;

    var TTDisassmbly = $("#Editor6").html();
    var TTDisassmbly1 = $.parseHTML(TTDisassmbly);
    obj.TTDisassmbly = TTDisassmbly1[1].innerHTML;

    var TTClean = $("#Editor7").html();
    var TTClean1 = $.parseHTML(TTClean);
    obj.TTClean = TTClean1[1].innerHTML;

    var TTAssmbly = $("#Editor8").html();
    var TTAssmbly1 = $.parseHTML(TTAssmbly);
    obj.TTAssmbly = TTAssmbly1[1].innerHTML;


    var TTFinalChk = $("#Editor9").html();
    var TTFinalChk1 = $.parseHTML(TTFinalChk);
    obj.TTFinalChk = TTFinalChk1[1].innerHTML;

    var TTToolKit = $("#Editor10").html();
    var TTToolKit1 = $.parseHTML(TTToolKit);
    obj.TTToolKit = TTToolKit1[1].innerHTML;

    var TTPolishing = $("#Editor11").html();
    var TTPolishing1 = $.parseHTML(TTPolishing);
    obj.TTPolishing = TTPolishing1[1].innerHTML;

    var TTHRNotes = $("#Editor12").html();
    var TTHRNotes1 = $.parseHTML(TTHRNotes);
    obj.TTHRNotes = TTHRNotes1[1].innerHTML;

    jQuery.ajaxSettings.traditional = true;

    $.ajax({
        type: 'POST',
        url: '/DetailMoldInfo/SaveTechTipsMoldSpec',
        data: obj,
        success: function (data) {

        },
        error: function () {

        }
    })
})

document.getElementById('inner_tab_content').style.height = window.innerHeight - 209 + 'px';
})
