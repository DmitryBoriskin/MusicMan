var $modal, $modalTitle, $modalBody, $modalFooter
//if (top != self) { top.location.href = location.href; }


$(document).ready(function () {
    $modal = $('.modal');
    $modalTitle = $('.modal .modal-title');
    $modalBody = $('.modal .modal-body');
    $modalFooter = $('.modal .modal-footer');

    $('#sb-slider').slicebox({
        orientation: 'h',
        cuboidsRandom: true,
        disperseFactor: 30,
        autoplay: true,
        interval: 3000,
    });

    $('.swipebox').swipebox();

    // Показываем сообщение в модальном окне
    $('.modal[data-show="true"]').modal('toggle');
    $('.modal-footer a[data-action=false]').bind({
        click: function () {
            $('.modal').modal('toggle');
            setCursor();

            return false;
        }
    })

    // 
    $('.video-inspector').bind({
        click: function (e) {
            var audios = $('audio');
            for (var i = 0; i < audios.length; i++) {
                audios[i].pause();
            }
            audios = $(this).parent().find('audio');
            for (var i = 0; i < audios.length; i++) {
                audios[i].play();
            }
            var videos = $('video');
            for (var i = 0; i < videos.length; i++) {
                videos[i].pause();
            }
            videos = $(this).parent().find('video');
            for (var i = 0; i < videos.length; i++) {
                videos[i].play();
            }

            $("iframe").each(function () {
                $(this)[0].contentWindow.postMessage('{"event":"command","func":"pauseVideo","args":""}', '*');
            });
            $('.video-inspector').css('display', 'block');
            

            $(this).css('display', 'none');
            $(this).parent().find('iframe')[0].contentWindow.postMessage('{"event":"command","func":"playVideo","args":""}', '*');
        }
    });

    //
    $('.new-item').bind({
        click: function () {
            var $form = $('.work-item-form');
           
            if ($form.attr('class') == 'work-item-form') {
                $form.addClass('active')
            }
            else {
                $form.removeClass('active');
                $('.add-buttons div').removeClass('active');
                $('.add-buttons input').css('display', 'none');
            }
        }
    });
    $('.add-photo, .add-video, .add-music').bind({
        click: function () {
            $('.add-buttons div').removeClass('active');
            $('.add-buttons input').css('display', 'none');
            $(this).addClass('active').find('input').css('display', 'block');
        }
    });

    //
    $('.work-item button').bind({
        click: function () {
            var req_count = $('form input[required]:invalid').length

            if (req_count == 0) {
                var $bg = $('<div/>', { 'class': 'prelouder' });

                $(this).closest(".work-item").append($bg);
            }
        }
    })

    // Удаление записи на стене пользователя
    $('.del-work a').bind({
        mousedown: function () {
            var link = $(this);
            clear_modal();

            var $BtnOk = $('<button/>', { 'id': 'modal-btn-ok', 'class': 'btn btn-danger' }).append('Да');
            $BtnOk.click(function () {
                link.trigger('click');
            })

            var $BtnNo = $('<button/>', { 'id': 'modal-btn-no', 'class': 'btn btn-default' }).append('Нет');
            $BtnNo.click(function () {
                $('.modal').modal('toggle');
            });

            $modalTitle.append('Уведомление');
            $modalBody.append('<p>Вы действительно хотите удалить запись?</p>');
            $modalFooter.append($BtnOk).append($BtnNo);

            $modal.modal('toggle');
            return false;
        },
        click: function () {
            var block = $(this).closest('.work-item');
            var idWork = $(this).attr("data-id");
            $.ajax({
                    type: "POST",
                    async: false,
                    url: "/del/",
                    data: { id: idWork },
                    error: function () { alert("error"); },
                    success: function (data) {
                        var _result = data.Result;

                        if (_result == 1) {
                            block.slideToggle(function () { block.remove(); });
                            $('.modal').modal('toggle');
                        }
                        else
                        {
                            clear_modal();
                            var $BtnOk = $('<button/>', { 'id': 'modal-btn-no', 'class': 'btn btn-danger' }).append('закрыть');
                            $BtnOk.click(function () {
                                $('.modal').modal('toggle');
                            });
                            $modalTitle.append('Ошибка');
                            $modalBody.append('<p>Удалить не получилось, попробуйте позже.</p>');
                            $modalFooter.append($BtnOk);
                        }
                    }
                });
        }
    });

    // Запрос телефона пользователя
    $('.getPhone').bind({
        click: function () {
            var $obj = $(this).empty(); 

            $.ajax({
                type: "POST",
                async: false,
                url: "/" + $obj.attr("data-user") + "/getPhone",
                error: function () { $obj.append("отсутствует"); },
                success: function (data) {
                    var _result = data.Result;
                    $obj.append(_result);
                }
            });
        }
    });

    // Запрос электронной почты пользователя
    $('.getMail').bind({
        click: function () {
            $(this).empty();
            $(this).addClass("icon-wait");

            var $obj = $(this);

            $.ajax({
                type: "POST",
                async: false,
                url: "/" + $obj.attr("data-user") + "/getMail",
                error: function () { $obj.append("отсутствует"); },
                success: function (data) {
                    var _result = data.Result;

                    $obj.append(_result);
                }
            });
        }
    });

    $('.like').bind({
        click: function () {
            var work_id = $(this).closest('.work-item').attr('id').replace('work-','')
            var $obj = $(this);

            $.ajax({
                type: "POST",
                async: false,
                url: '/like/' + work_id + '/',
                error: function () {  },
                success: function (data) {
                    $obj.empty();
                    var _count = data.Сount;
                    var _status = data.Status;
                    var _result = data.Result;

                    $obj.removeClass('active').addClass(_status);
                    $obj.append("  " + _count);
                    $obj.append("  " + _result);
                }
            });
        }
    });

    // инициализаация полей даты
    $('input[data-type=date').datepicker({ onSelect: function (dateText, inst) { $(this).attr('value', dateText); } });
    //Вызов плагина маски ввода
    $('input[data-mask]').each(function () {
        $(this).mask($(this).attr('data-mask'));
    });
});


// Очищаем модальное окно
function clear_modal() {
    $modal.find('.modal-dialog').removeClass().addClass('modal-dialog'),
    $modalTitle.empty();
    $modalBody.empty();
    $modalFooter.empty();
}


// Всплывающие окна
function Confirm(Title, Body, Object) {
    clear_modal();

    var $BtnOk = $('<button/>', { 'id': 'modal-btn-ok', 'class': 'btn btn-danger' }).append('Да');
    $BtnOk.click(function () {
        $('form input[required]').removeAttr('required');
        Object.trigger('click');
    })

    var $BtnNo = $('<button/>', { 'id': 'modal-btn-no', 'class': 'btn btn-default' }).append('Нет');
    $BtnNo.click(function () {
        $('.modal').modal('toggle');
    });

    $modalTitle.append(Title);
    $modalBody.append('<p>' + Body + '</p>');
    $modalFooter.append($BtnOk).append($BtnNo);

    $modal.modal('toggle');
}
