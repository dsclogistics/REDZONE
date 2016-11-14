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
        //alert("The Server has requested a password Encryption using a random one-time-use token [" + iv + "]");
        //var ciphertext = Aes.Ctr.encrypt($('#pwd').val(), iv, 256);  //Using AES-CTR

        //Before subittting the Login Form, encrypt the password field with the key just received from the server
        //var key = CryptoJS.enc.Utf8.parse('8080808080808080');    //Default Key
        //var iv = CryptoJS.enc.Utf8.parse('8080808080808080');     //Default Key
        var key = CryptoJS.enc.Utf8.parse(iv);
        var iv = CryptoJS.enc.Utf8.parse(iv);
        var ciphertext = CryptoJS.AES.encrypt(CryptoJS.enc.Utf8.parse($('#pwd').val()), key, {
            keySize: 128 / 8,
            iv: iv,
            mode: CryptoJS.mode.CBC,
            padding: CryptoJS.pad.Pkcs7
        });

        //var message = '<div style="text-align:center">The server you are trying to access has requested to encrypt your credential using a single-use encryption key:<div>' + iv +
        //              '</div></div><div><br />Submiting Encrypted Credentials:</div><div>' + ciphertext + '</div>';
        //var messagePlain = '\nThe server you are trying to access has requested to encrypt your credentials using a single-use encryption key:\n' + iv +
        //              '\n\n\nSubmiting Encrypted Credentials:\n' + ciphertext + '\n';
        //alert(messagePlain);

        //alert("Your password has ben encrypted before submission:\nPassword: '" + ciphertext + "'\n will be submitted.");
        //alert('Encrypting Form...\nPassword: ' + $("#pwd").val() + " encrypted with token '" + iv + "' => #" + ciphertext + "#");
        $('#pwd').val(Array($('#pwd').val().length + 1).join("X"));
        //alert("New password is: " + $('#pwd').val());
        $('#uPWD').val(ciphertext);
        //alert("Hidden Field Value is: " + $('#uPWD').val());
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
            $('#btnRZLogin').prop("disabled", true);
            //submit the Login Credentials
            submitEncry();
        }        
    });

    $('#btnRZLogin').click(function () {
        //Submit the Login Credentials
        $('#btnRZLogin').prop("disabled", true);
        submitEncry();
    });

});