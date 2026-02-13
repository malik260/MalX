var changePasswordData = {};
function changePassValidtion() {
    inputValidation = [];
    inputValidation.push(validateField('', "old_password"));
    inputValidation.push(validateField('', "new_password"));
    inputValidation.push(validateField('', "cnew_password"));
    if (inputValidation.includes(false)) {
        return false;
    }
    changePasswordData.OldPassword = $('#old_password').val();
    changePasswordData.NewPassword = $('#new_password').val();
    changePasswordData.CNewPassword = $('#cnew_password').val();
    return true;
}


function popChangePasswordModal(id) {
    $('#changePassModal').modal('show');
}

function changePassword() {
    if (changePassValidtion()) {
        if (changePasswordData.CNewPassword !== changePasswordData.NewPassword) {
            errorAlert("New Password and Confirm Password Not Matched");
            return false;
        }
        processingBtn('change_pass_btn');
        let details = JSON.stringify(changePasswordData);
        $.ajax({
            type: 'Post',
            url: "/Account/ChangePassword",
            dataType: 'json',
            data: { model: details },
            success: function (result) {
                if (!result.isError) {
                    successAlert(result.message);
                    returnDefaultBtn('change_pass_btn');
                    $('#changePassModal').modal('hide');
                }
                else {
                    returnDefaultBtn('change_pass_btn');
                    errorAlert(result.message);
                }
            },
            error: function (ex) {
                returnDefaultBtn('change_pass_btn')
                errorAlert("Network failure, please try again " + ex);
            }
        });
    }
}





