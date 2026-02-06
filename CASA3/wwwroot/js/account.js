//Login Section

var staffLoginData = {};
function staffLoginValidtion() {
    inputValidation = [];
    inputValidation.push(validateEmail('email'));
    inputValidation.push(validateField('', "password"));
    if (inputValidation.includes(false)) {
        return false;
    }
    staffLoginData.Email = $('#email').val();
    staffLoginData.Password = $('#password').val();
    return true;
}
function staffLogin() {
    if (staffLoginValidtion()) {
        processingBtn('staff_login_btn');
        let loginDetails = JSON.stringify(staffLoginData);
        $.ajax({
            type: 'Post',
            url: "/Account/StaffLogin",
            dataType: 'json',
            data: { model: loginDetails },
            success: function (result) {
                if (!result.isError) {
                    successAlertWithRedirect(result.message, "/Admin");
                }
                else {
                    returnDefaultBtn('staff_login_btn');
                    errorAlert(result.message);
                }
            },
            error: function (ex) {
                returnDefaultBtn('staff_login_btn')
                errorAlert("Network failure, please try again " + ex);
            }
        });
    }
}


//Login Section Ended




