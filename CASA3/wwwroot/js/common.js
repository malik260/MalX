//<---- ConvertPictureToBase64 function will convert image to base64 and store in var base64ProfilePix---->
var base64ProfilePix = "";
var btnDefaultText = "";
var fileName = "";
function ConvertPictureToBase64(inputId) {
    var fileObj = document.getElementById(inputId).files;
    if (fileObj[0] != null) {
        const reader = new FileReader();
        reader.readAsDataURL(fileObj[0]);
        reader.onload = function () {
            base64ProfilePix = reader.result;
            fileName = fileObj[0].name;
            $("#selectedSupportFile").text(base64ProfilePix);
            return
        }
    } else {
        base64ProfilePix = "";
    }
}
// $(document).ready(function () {
//     $('.mtg-select2').select2();
// });
//<---- For email vaildation---->
function validateEmail(inputId) {
    const emailInput = document.getElementById(inputId);
    const email = emailInput.value.trim();
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

    if (email === "") {
        $('#' + inputId).css('border', 'solid 2px red');
        return false;
    }

    if (!emailRegex.test(email)) {
        $('#' + inputId).css('border', 'solid 2px red');
        return false;
    }
    $('#' + inputId).css('border', 'solid 1px #ccc');
    return true;
}

function processingBtn(btnId) {
    btnDefaultText = $('#' + btnId).html();
    $('#' + btnId).html("Please wait...");
    $('#' + btnId).attr("disabled", true);
}

function returnDefaultBtn(btnId) {
    $('#' + btnId).html(btnDefaultText);
    $('#' + btnId).attr("disabled", false);
}

//<---- General Function start here---->
var inputValidation = [];
function validateField(i, fieldName) {
    let value = $("#" + fieldName + i).val();
    if (value != undefined && value.trim() === "") {
        $("#" + fieldName + i).css('border', 'solid 2px red');
        return false; // Validation failed
    } else {
        $("#" + fieldName + i).css('border', 'solid 1px #ccc');
        return true; // Validation successful
    }
}

var guidMiniValue = "00000000-0000-0000-0000-000000000000";
function validateGuidField(i, fieldName) {
    let value = $("#" + fieldName + i).val();
    if (value != undefined && value.trim() === guidMiniValue) {
        $("#" + fieldName + i).css('border', 'solid 2px red');
        return false; // Validation failed
    } else {
        $("#" + fieldName + i).css('border', 'solid 1px #ccc');
        return true; // Validation successful
    }
}
function validateNumericField(i, fieldName) {
    let value = $("#" + fieldName + i).val();
    if (value != undefined && (isNaN(value) || value <= 0)) {
        $("#" + fieldName + i).css('border', 'solid 2px red');
        return false; // Validation failed
    } else {
        $("#" + fieldName + i).css('border', 'solid 1px #ccc');
        return true; // Validation successful
    }
}
function validateGuidSelect2Field(i, fieldName) {
    let value = $("#" + fieldName + i).val();
    if (value != undefined && value.trim() === guidMiniValue) {
        // Target the select2 container
        $("#" + fieldName + i).next('.select2-container').find('.select2-selection').css('border', 'solid 2px red');
        return false; // Validation failed
    } else {
        // Target the select2 container
        $("#" + fieldName + i).next('.select2-container').find('.select2-selection').css('border', 'solid 1px #ccc');
        return true; // Validation successful
    }
}
function validateNumericSelect2Field(i, fieldName) {
    let value = $("#" + fieldName + i).val();
    if (value != undefined && (isNaN(value) || value <= 0)) {
        $("#" + fieldName + i).next('.select2-container').find('.select2-selection').css('border', 'solid 2px red');
        return false; // Validation failed
    } else {
        $("#" + fieldName + i).next('.select2-container').find('.select2-selection').css('border', 'solid 1px #ccc');
        return true; // Validation successful
    }
}
function validateStringSelect2Field(i, fieldName) {
    let value = $("#" + fieldName + i).val();
    if (value != undefined && value.trim() === "") {
        $("#" + fieldName + i).next('.select2-container').find('.select2-selection').css('border', 'solid 2px red');
        return false; // Validation failed
    } else {
        $("#" + fieldName + i).next('.select2-container').find('.select2-selection').css('border', 'solid 1px #ccc');
        return true; // Validation successful
    }
}
function dataTableTrigger() {
    if ($('.datatable').length > 0) {
        $('.datatable').DataTable({
            "bFilter": false,
        });
    }
}
function getDataForDelete(id) {
    $('#delete_Id').val(id);
    $('#deleteModal').modal('show');
}