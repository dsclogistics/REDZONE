function submitEncry(e) {
    //Retrieve an encryption IV from Server and use it to login the user with encrypted credentials
    $.ajax({
        url: '/Account/getIV',
        method: "GET",
        cache: false,
        //data: { raw_json: jsonPayload },
        //contentType: "application/json; charset=utf-8",
        //dataType: "json",
        error: function (jqXHR, textStatus, errorThrown) {
            showAlert("Warning. Server cannot be contacted\nError:" + textStatus + "," + errorThrown);  //<-- Trap and alert of any errors if they occurred
        }
    }).done(function (iv) {
        alert("The Server has requested a password Encryption using a random one-time-use token [" + iv + "]");
        var ciphertext = Aes.Ctr.encrypt($('#pwd').val(), iv, 256);
        alert("Your password has ben encrypted before submission:\nPassword: '" + ciphertext + "'\n was submitted.");
        //alert('Encrypting Form...\nPassword: ' + $("#pwd").val() + " encrypted with token '" + iv + "' => #" + ciphertext + "#");

        //If everything is OK, submit the form.
        //document.forms['myform'].submit();
        //document.forms[0].submit();
        $("#frmLogin").submit();
    });
}


$(document).ready(function () {
    //document.onkeypress(function () {
    //    alert("Test");
    //});
    $("#frmLogin").keypress(function (e) {
        if (e.which == 13) {
            //submit the Login Credentials
            submitEncry();
        }        
    });

    $('#btnLogin').click(function () {
        //Submit the Login Credentials
        submitEncry();
    });

});