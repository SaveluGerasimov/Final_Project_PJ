// site.js

$(document).ready(function() {
    // Инициализация тултипов
    $('[data-toggle="tooltip"]').tooltip({
        trigger: 'hover'
    });
    
    // Подтверждение удаления
    $('.delete-confirm').on('click', function(e) {
        if (!confirm('Вы уверены, что хотите удалить этот элемент?')) {
            e.preventDefault();
            return false;
        }
    });
    
    // Подсветка активной ссылки в навигации
    var currentUrl = window.location.pathname;
    $('.navbar-nav .nav-link').each(function() {
        var linkUrl = $(this).attr('href');
        if (linkUrl && currentUrl.indexOf(linkUrl) !== -1) {
            $(this).addClass('active');
        }
    });
    
    // Анимация кнопок при наведении
    $('.btn').hover(
        function() {
            $(this).css('transform', 'translateY(-2px)');
        },
        function() {
            $(this).css('transform', 'translateY(0)');
        }
    );
    
    // Плавная прокрутка для якорных ссылок
    $('a[href^="#"]').on('click', function(event) {
        if (this.hash !== "") {
            event.preventDefault();
            var hash = this.hash;
            $('html, body').animate({
                scrollTop: $(hash).offset().top - 70
            }, 800);
        }
    });
    
    // Динамическое обновление количества символов в текстовых полях
    $('textarea[maxlength]').each(function() {
        var maxLength = $(this).attr('maxlength');
        var $counter = $('<div class="text-muted small mt-1">Осталось символов: <span class="char-count">' + maxLength + '</span></div>');
        $(this).after($counter);
        
        $(this).on('input', function() {
            var currentLength = $(this).val().length;
            var remaining = maxLength - currentLength;
            $(this).next('.char-count').text(remaining);
            
            if (remaining < 50) {
                $(this).next().find('.char-count').addClass('text-warning');
            } else {
                $(this).next().find('.char-count').removeClass('text-warning');
            }
        }).trigger('input');
    });
});