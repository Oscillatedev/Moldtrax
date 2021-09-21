function CreateTrobleShooter() {
    $.ajax({
        url: '/TroubleShooterGuide/Create',
        type: 'Get',
        success: function (data) {
            $("#TroubleShooterHtml").html(data);
            $("#TrbleDiv").hide();
            $(".editable").attr('data-placeholder', '');
            //$("#AddTroubleShtr").hide();
            //$("#EditTroubleShtr").hide();
            //$("#DeleteTroubleShtr").hide();
        },

        error: function () { }

    });
}

$("#SaveTxtbtn").click(function () {
    debugger;
    var data = $(".note-editable").html();
    $('[name=NewDDID]').html(data);
    $('[name=NewDDID2]').html(data);
    $('[name=NewDDID2]').attr('name', '');
    $('[name=NewDDID]').attr('name', '');
    $(".close1").click();
    $(".popover-content").hide();
});

$(".del_btn").click(function () {
    $('[name=NewDDID2]').attr('name', '');
    $('[name=NewDDID]').attr('name', '');
    $(".close1").click();
});




$(".editor1c").dblclick(function () {
    debugger;
    $(".editable").attr('data-placeholder', '');
    //$(".modal-header").html('MoldTrax');
    var row = $(this);
    var dd = $(this).closest("tr").find("td:eq(3)").find("#editor1").html();
    var newdata = $.parseHTML(dd);
    $("#Showtxtbtn").click();
    $(".note-editable").html(newdata[1].innerHTML);
    $(this).closest("tr").find("td:eq(3)").find("#TSExplanation").attr('name', 'NewDDID');
    $(this).closest("tr").find("td:eq(3)").find(".editable").attr('name', 'NewDDID2');
    if (newdata[1].innerHTML === "") {
        $('.note-editable').trigger('focus');
    }
    else
    {
        $('.note-editable').placeCursorAtEnd();
    }
});


$(".editor2c").dblclick(function () {
    debugger;
    var row = $(this);
    $(".editable").attr('data-placeholder', '');
    var dd = $(this).closest("tr").find("td:eq(4)").find("#editor2").html();
    //var dd = $(this).closest("tr").find("td:eq(4)").find("#TSProbCause").val();
    var newdata = $.parseHTML(dd);

    $("#Showtxtbtn").click();
    //$("#ShowTxtVal").attr('data-placeholder', '');
    $(".note-editable").html(newdata[1].innerHTML);
    $(this).closest("tr").find("td:eq(4)").find("#TSProbCause").attr('name', 'NewDDID');
    $(this).closest("tr").find("td:eq(4)").find(".editable").attr('name', 'NewDDID2');
    //row.eq(0).find(".editable").attr('name', 'NewDDID');
    if (newdata[1].innerHTML === "") {
        $('.note-editable').trigger('focus');
    }
    else {
        $('.note-editable').placeCursorAtEnd();
    }
});


$(".editor3c").dblclick(function () {
    debugger;
    var row = $(this);
    $(".editable").attr('data-placeholder', '');
    var dd = $(this).closest("tr").find("td:eq(5)").find("#editor3").html();
    //var dd = $(this).closest("tr").find("td:eq(5)").find("#TSSolution").val();
    var newdata = $.parseHTML(dd);
    $("#Showtxtbtn").click();
    $(".note-editable").html(newdata[1].innerHTML);
    $(this).closest("tr").find("td:eq(5)").find("#TSSolution").attr('name', 'NewDDID');
    $(this).closest("tr").find("td:eq(5)").find(".editable").attr('name', 'NewDDID2');
    if (newdata[1].innerHTML === "") {
        $('.note-editable').trigger('focus');
    }
    else {
        $('.note-editable').placeCursorAtEnd();
    }
    //$("#ShowTxtVal").attr('data-placeholder', '');
    //$(".note-editable").html(dd);
    //row.eq(0).find(".editable").attr('name', 'NewDDID');
});

$(".editor4c").dblclick(function () {
    debugger;
    var row = $(this);
    $(".editable").attr('data-placeholder', '');
    var dd = $(this).closest("tr").find("td:eq(6)").find("#editor4").html();
    var newdata = $.parseHTML(dd);
    //var dd = $(this).closest("tr").find("td:eq(6)").find("#TSPreventAction").val();
    $("#Showtxtbtn").click();
    $(".note-editable").html(newdata[1].innerHTML);
    $(this).closest("tr").find("td:eq(6)").find("#TSPreventAction").attr('name', 'NewDDID');
    $(this).closest("tr").find("td:eq(6)").find(".editable").attr('name', 'NewDDID2');
    if (newdata[1].innerHTML === "") {
        $('.note-editable').trigger('focus');
    }
    else {
        $('.note-editable').placeCursorAtEnd();
    }
    //$("#ShowTxtVal").attr('data-placeholder', '');
    //$(".note-editable").html(dd);
    //row.eq(0).find(".editable").attr('name', 'NewDDID');
});



$(".MyFileCont").hide();
$(".add_btn").click(function () {
    $(this).closest("tr").find('td:eq(1)').find("#AddImg").click();
});

$(".del_btn").click(function () {
    debugger;
    $(this).closest("tr").find('td:eq(2)').find("#UploadedFile").attr('src', '').fadeIn();
    var TsGuide = $(this).closest("tr").find('td:eq(1)').find("#TSGuide").val();

    $.ajax({
        url: '/DetailMoldInfo/DeleteImage?TsGuide=' + TsGuide,
        type: 'Post',
        success: function () { },
        error: function () { }
    });
    //$('#UploadedFile').attr('src', '').fadeIn();
});


var ColorPickerExtension = MediumEditor.extensions.button.extend({
    name: "colorPicker",
    action: "applyForeColor",
    aria: "color picker",
    contentDefault: "<span class='editor-color-picker'>Text Color<span>",

    init: function () {
        this.button = this.document.createElement('button');
        this.button.classList.add('medium-editor-action');
        this.button.innerHTML = '<b>C</b>';
        initPicker(this.button);
    }
});

var pickerExtension = new ColorPickerExtension();

function setColor(color) {
    pickerExtension.base.importSelection(this.selectionState);
    pickerExtension.document.execCommand("styleWithCSS", false, true);
    pickerExtension.document.execCommand("foreColor", false, color);
}

function initPicker(element) {
    $(element).spectrum({
        allowEmpty: true,
        color: "#f00",
        showInput: true,
        showAlpha: true,
        showPalette: true,
        showInitial: true,
        hideAfterPaletteSelect: false,
        preferredFormat: "hex3",
        change: function (color) {
            setColor(color);
        },
        hide: function (color) {
            //applyColor(color);
        },
        palette: [
            ["#000", "#444", "#666", "#999", "#ccc", "#eee", "#f3f3f3", "#fff"],
            ["#f00", "#f90", "#ff0", "#0f0", "#0ff", "#00f", "#90f", "#f0f"],
            ["#f4cccc", "#fce5cd", "#fff2cc", "#d9ead3", "#d0e0e3", "#cfe2f3", "#d9d2e9", "#ead1dc"],
            ["#ea9999", "#f9cb9c", "#ffe599", "#b6d7a8", "#a2c4c9", "#9fc5e8", "#b4a7d6", "#d5a6bd"],
            ["#e06666", "#f6b26b", "#ffd966", "#93c47d", "#76a5af", "#6fa8dc", "#8e7cc3", "#c27ba0"],
            ["#c00", "#e69138", "#f1c232", "#6aa84f", "#45818e", "#3d85c6", "#674ea7", "#a64d79"],
            ["#900", "#b45f06", "#bf9000", "#38761d", "#134f5c", "#0b5394", "#351c75", "#741b47"],
            ["#600", "#783f04", "#7f6000", "#274e13", "#0c343d", "#073763", "#20124d", "#4c1130"]
        ]
    });
}
//ColorPickerExtension = MediumEditor.util.derives(MediumEditor.statics.DefaultButton, ColorPickerDerived);

var editor = new MediumEditor('.editable', {
    toolbar: {
        buttons: ['bold', 'italic', 'underline', 'anchor', 'colorPicker'],
        allowMultiParagraphSelection: 'true',
        spellcheck: 'false'
    },
    extensions: {
        'colorPicker': pickerExtension
    },
    keyboardCommands: false,
});



$("#table_troubleCreate tbody tr").each(function () {
    var row = $(this);
    var Trouble;
    Trouble = row.find("TD").eq(1).find("#TsTypeVal").val();
    row.find("TD").eq(1).find("#TSType").val(Trouble);
});




//$(document).ready(function () {
    //$("#QuoteTable").dynatable({
    //    table: {
    //        defaultColumnIdStyle: 'trimDash'
    //    }
    //});
    //$('.textarea-editor').summernote({
    //    height: 300, // set editor height
    //    minHeight: null, // set minimum height of editor
    //    maxHeight: null, // set maximum height of editor
    //    focus: true // set focus to editable area after initializing summernote
    //});

    $('#Quotecheckall').click(function () {
        debugger;
        if ($(this).is(':checked'))
            $('input[name="checkname"].Subcheckbox').prop('checked', true);
        else
            $('input[name="checkname"].Subcheckbox').prop('checked', false);

        $('input[name="checkname"].Subcheckbox').click(function () {
            debugger;
            if ($('input:checkbox[name="checkname"].Subcheckbox').length === $('input:checkbox[name="checkname"].Subcheckbox:checked').length) {
                $('input[name="checkname"].SelectAll').prop("checked", true);
            }
            else {
                $('input[name="checkname"].SelectAll').prop("checked", false);
            }

        });

    });


function DeleteSelectedTroubleShooter() {
    debugger;
    //$(".loader__wrap").show();
    var MoldID = $("#MainMoldHidnID").val();
    var selID = new Array();
    var str = '';
    //$("#loader").show();
    $('input:checkbox[name=checkname].Subcheckbox:checked').each(function () {
        if ($(this).prop('checked')) {
            var selID = $(this).val();
            if (selID === "on")
                selID = 0;
            if (selID > 0) {
                str += "" + selID + ",";
            }
            debugger;
        }
    });

    if (str === '') {
        Swal.fire('Please select item.')
    }

    else {
        Swal.fire({
            title: 'Are you sure?',
            text: "You won't be able to revert this.",
            type: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, delete it.'
        }).then((result) => {
            if (result.value) {
                $(".loader__wrap").show();
                $.ajax({
                    url: "/DetailMoldInfo/DeleteTrouble",
                    type: "post",
                    data: { str: str, MoldID: MoldID },
                    success: function (data) {
                        debugger;
                        //$(".loader__wrap").hide();
                        Swal.fire({
                            title: 'Deleted',
                            text: 'Data has been deleted.',
                            type: "success"
                        }).then(function () {
                            $("#MoldTb").html(data);
                        })
                        //window.location.href = '/DetailMoldInfo/Index';
                    },
                    error: function () {
                        //$(".loader__wrap").hide();
                    }
                });
            }
        })
    }
    debugger;
}



$(document).ready(function () {
    // Initialize Editor

});

//var modal = document.getElementById("imgModal");
//debugger;
//// Get the image and insert it inside the modal - use its "alt" text as a caption
//var img = document.getElementsByClassName("ImageSrc");
//var modalImg = document.getElementsByClassName("modal-content");
//var captionText = document.getElementById("caption");
//img.onclick = function () {
//    modal.style.display = "block";
//    modalImg.src = this.src;
//    captionText.innerHTML = this.alt;
//}

var modal = document.getElementById("imgModal");

$(".ImageSrc").click(function () {
    debugger;
    var modalImg = document.getElementById("img01");
    modal.style.display = "block";
    modalImg.src = this.src;
    //captionText.innerHTML = this.alt;
})

// Get the <span> element that closes the modal
var span = document.getElementsByClassName("close")[0];

// When the user clicks on <span> (x), close the modal
    span.onclick = function () {
        modal.style.display = "none";
    };