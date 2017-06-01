$(function init() {
    $('#fmrLogin').submit(function (event) {
        $('#Progres').modal('show');
        event.preventDefault();
        validar($('#email').val(), $('#password').val());
    });

    function validar(email, pass) {
        var ip = myip;
        $.ajax({
            type: "get",
            url: "api/usuarios/login",
            data: { email: email, pass: pass, IP: ip },
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            success: function (data) {
                if (data === null) {
                    alertify.alert('Sistema', 'Credenciales Incorrectas');
                    $('#Progres').modal('hide');
                } else {
                    $('#Progres').modal('hide');
                    localStorage.setItem("Id", data);
                    localStorage.setItem("Ip", ip);
                    location.href = "views/Home/Home.html";
                }
            }
        });    
    }
});