$(function init() {
    sesion();
    function sesion() {
        var id = localStorage.getItem('Id');
        if (id === null) {
            location.href = "../../index.html";
        } else {
            $('#Progres').modal('show');
            TraerMenu(id);
        }
    }

    $('#Salir').click(function () {
        var id = localStorage.getItem('Id');
        $.ajax({
            type: "PUT",
            url: "../../api/usuarios/usuario",
            data: { Id: id },
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
        }).responseText;
        location.href = "../../index.html";
    });

    function TraerMenu(usuario) {
        $.ajax({
            type: "GET",
            url: "../../api/usuarios/TraerMenu",
            data: { UsuarioId: usuario },
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            success: function (data) {
                if (data !== null) {
                    ProcesarMenu(data);
                } else {
                    location.href = "../../index.html";
                }
            }
        });
    }

    function ProcesarMenu(Menu) {
        var MenuT = "";
        $(Menu).each(function (a, b) {
            var li = "<li class='dropdown'>" +
                "<a class='dropdown-toggle' id='" + "subMenu" + a + "' role='button' data-toggle='dropdown' aria-haspopup='true' aria-expanded='false'>"+
            b.Descripcion + "<span class='caret'></span> </a>" +
                "<ul class='dropdown-menu' aria-labelledby='" + "subMenu" + a + "'>";
            $(b.Pantallas).each(function (a1, b1) {
                li += "<li><a class='dropdown-item' href='" + b1.DireccionPantalla+"'>" + b1.Descripcion + " <span class='" + b1.Icono + "'></span></a>" +
                    "</li >";
            });
            li += "</ul></li>";
            MenuT += li;
        });
        $('#menuitems').append(MenuT);
        $('#Progres').modal('hide');
    }

});