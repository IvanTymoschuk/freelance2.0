
   function ShowPartailBan() {
        let info = $("#OpenBan").data("value");
        Metro.dialog.open('#demoDialog1');
        $("#UidInBP").val(info);
    }
    function ShowPartailRoles() {
        setTimeout(function () {
            //let info = $("#OpenRoles").data("value");
            Metro.dialog.open('#demoDialog2');
            // $("#UidInRP").val(info);
        }, 10);
    }
    function UnBanShow() {
        var notify = Metro.notify;
        notify.create("User Unbanned!", null, {

        });
    };
   
    function RolesUpdated() {
        var notify = Metro.notify;
        notify.create("Roles has been updated!!!", null, {
        });
    };

    function ErrorJS() {
        Metro.infobox.create('<h3>SERVER ERROR<h3/><br><p>1. No Internet<br>2. Server error<p/>', 'alert');
    }
   $(function() {
        $("body").remove("#overlay");
        $("#preloader").css({'display':'none'});
    });
    function LoaderJS() {
        var docHeight = $(document).height();
        $("body").append("<div id='overlay'></div>");
        $("#preloader").css({'display':'block'});
        $("#overlay").height(docHeight)
            .css({
                "display": "block",
                'opacity': 0.4,
                'position': 'absolute',
                'top': 0,
                'left': 0,
                'background-color': 'black',
                'width': '100%',
                'z-index': 5000
            });
  
    };
    function HideLoaderJS() {
        $("body").remove("#overlay");
        $("#overlay").css({ "display": "none" });
        $("#preloader").css({'display':'none'});
    }