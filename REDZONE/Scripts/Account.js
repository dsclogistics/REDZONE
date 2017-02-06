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
        $('#pwd').val(Array($('#pwd').val().length + 1).join("X"));   //Mask the plaintext passwords by replacing all characters with "X"
        //alert("New password is: " + $('#pwd').val());
        $('#uEncryPwd').val(ciphertext);                                 //This is the real password that is submitted to the server in encripted format
        //alert("Hidden Field Value is: " + $('#uPWD').val());
        //If everything is OK, submit the form.
        //document.forms['myform'].submit();
        //document.forms[0].submit();
        //alert("User Name: " + $('#UserName').val() + "\nUser SSO: " + $('#userSSO').val() + "\nDomain: " + $('#domain').val() + "\nPassword: " + $('#uEncryPwd').val());

        $("#frmLogin").submit();
    });
}

function submitLoginForm() {
    $('#UserName').val($('#UserName').val().trim());
    //Verify all fields have been filled out
    if ($('#UserName').val() == "") {
        $('#erroMsgDIV').html("* User Name is Required");
        $('#UserName').select().focus();
        return;
    }

    if ($('#pwd').val() == "") {
        $('#erroMsgDIV').html("* Password cannot be blank");
        $('#pwd').select().focus();
        return;
    }

    //Submit the Login Credentials
    
    //$('#btnRZLogin').prop("disabled", true);
    $('#btnRZLogin').button("loading");
    //Set the domain and User SSO hidden fields for submission

    var userSSO = $('#UserName').val();
    var domainChar = userSSO.substring(0, 1);
    var uDomain = "";
    var hasDomainChar = true;
    switch (domainChar) {
        case '#':
            uDomain = "ColonialHeights";
            userSSO = $('#UserName').val().substring(1);
            break;
        case '@':
            uDomain = "dsccorp";
            userSSO = $('#UserName').val().substring(1);
            break;
        default:
            uDomain = "dsclogistics";
            hasDomainChar = false;
            break;
    }
    if (!hasDomainChar) {//User Name does not have a domain character. Look for "\" domain delimiter
        var delimiterIndex = userSSO.indexOf("\\");
        if (!(delimiterIndex == -1)) {//Domain delimiter was found
            uDomain = userSSO.substring(0, delimiterIndex);
            if (delimiterIndex + 1 == userSSO.length) {
                $('#erroMsgDIV').html("You must input a value after the slash");
                $('#UserName').select().focus();
                $('#btnRZLogin').prop("disabled", false);
                return;
            }
            else {
                userSSO = userSSO.substring(delimiterIndex + 1);
            }
        }
        //if (userSSO.includes("\\")) {
        //    alert("Domain delimiter found at index " + userSSO.indexOf("\\"));
        //}
    }
    else {  //If a Domain Formatting character was used, the "slash" option is not allowed
        if (!(userSSO.indexOf("\\") == -1)) {
            $('#erroMsgDIV').html("User Name Format is not Valid. Check your input");
            $('#UserName').select().focus();
            $('#btnRZLogin').prop("disabled", false);
            return;
        }
    }
    uDomain = uDomain.toUpperCase();
    if (!(uDomain == "DSCLOGISTICS" || uDomain == "DSCCORP" || uDomain == "COLONIALHEIGHTS")) {
        $('#erroMsgDIV').html("The Selected Domain is not Valid.");
        $('#UserName').select().focus();
        $('#btnRZLogin').prop("disabled", false);
        return;
    }

    $('#userSSO').val(userSSO);
    $('#domain').val(uDomain);
    
    //alert("User Name: " + $('#UserName').val() + "\nUser SSO: " + $('#userSSO').val() + "\nDomain: " + $('#domain').val() + "\nPassword: " + $('#uEncryPwd').val());

    submitEncry();
}

$(document).ready(function () {
    //document.onkeypress(function () {
    //    alert("Test");
    //});
    $("#frmLogin").keypress(function (e) {
        if (e.which == 13) {
            //submit the Login Credentials
            submitLoginForm();
        }
        else {
            $('#erroMsgDIV').html("");
            $('#btnRZLogin').button("reset");
        }
    });

    $('#btnRZLogin').click(function () {
        submitLoginForm();
    });

});