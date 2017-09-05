$(function () {
    'use strict';

    $("#localPatitionsList").on('click', 'form input[type="checkbox"]', function (e) {
        var current_target = $(e.currentTarget);
        var data = current_target.closest('form').serialize();
        if (current_target.is(':checked')) {
            $.post('/LocalPartition/Add', data, function (res) {
                if (res !== true) {
                    current_target.checked = false;
                }
                else if (current_target.closest('ul').children().length === current_target.closest('ul').find('input:checked').length) {
                    current_target.closest('ul').prev().find('input')[0].checked = true;
                }
            });
        }
        else {
            $.get('/LocalPartition/Delete', data, function (res) {
                if (res !== true) {
                    current_target.checked = true;
                }
                else {
                    current_target.closest('ul').prev().find('input')[0].checked = false;
                }
            });
        }
    });
    $(document).on('click', 'input[name="main_checkbox"]', function (e) {
        var current_target = $(e.currentTarget);
        if (current_target.is(':checked')) {
            $('input[type="checkbox"]:not(:checked)', current_target.closest('label').parent().next()).each(function (index, element) {
                $(element).click();
            });
        }
        else {
            $('input[type="checkbox"]:checked', current_target.closest('label').parent().next()).each(function (index, element) {
                $(element).click();
            });
        }
    });
});

function CheckAllCheckLocalPartition(obj) {
    var all = 0;
    var check = 0;
    $(obj).closest('tr').find('input[class="SelectedLocalPartition"]').each(function () {
        all++;
        if ($(this).prop('checked')) {
            check++;
        }
    });
    if (all === check) {
        $(obj).closest('tr').prev().find('input[name="btSelectItem"]').prop('checked', 'checked');
    } else {
        $(obj).closest('tr').prev().find('input[name="btSelectItem"]').prop('checked', '');
    }
}

function ChangeCheckLocalPartition(obj, controller, method) {

    var data = { ControllerName: controller, ActionName: method };

    $('.SelectedLocalPartition').attr("disabled", true);

    if ($(obj).prop('checked')) {
        $.post('/LocalPartition/Add', data, function (res) {
            if (res) {
                console.log('Сохранено, ' + controller + ", " + method);
            }
            $('.SelectedLocalPartition').attr("disabled", false);
        });
    } else {
        $.get('/LocalPartition/GetRoleLocalPartitionCount', data, function (res) {
            if (res >= 1) {
                if (confirm("Данный доступ используется в других ролях. Вы действительно хотите убрать этот доуступ?")) {
                    $.get('/LocalPartition/Delete', data, function (res) {
                        if (res) {
                            console.log('Удалено, ' + controller + ", " + method);
                        }
                        $('.SelectedLocalPartition').attr("disabled", false);
                    });
                } else {
                    obj.checked = !obj.checked;
                    CheckAllCheckLocalPartition(obj);
                }
            } else {
                $.get('/LocalPartition/Delete', data, function (res) {
                    if (res) {
                        console.log('Удалено, ' + controller + ", " + method);
                    }
                    $('.SelectedLocalPartition').attr("disabled", false);
                });
            }
            $('.SelectedLocalPartition').attr("disabled", false);
        });

        $('.SelectedLocalPartition').attr("disabled", false);
    }
    CheckAllCheckLocalPartition(obj);
}

$('#controllers_data_table').on('expand-row.bs.table', function (e, index, row, detail) {
    var controllersContent = '<b>Список пуст</b>';
    var roleId = $('#RoleId').val();
    var data = { roleId: roleId, _controller: row.controller };
    $.get('/LocalPartition/_MethodListPartial', data, function (res) {
        if (res) {
            controllersContent = res;
        }
        $(detail).html(controllersContent);
    });
});

$('#controllers_data_table').on('check.bs.table uncheck.bs.table ' +
    'check-all.bs.table uncheck-all.bs.table', function (e, rows, $element) {

        var data = { _controller: rows.controller };
        if ($element.prop('checked')) {
            $.get('/LocalPartition/AddActionsController', data, function (res) {
                if (res) {
                    console.log('_AddControllerMethodsPartial')
                }
            });
        }
        else {
            $.get('/LocalPartition/DeleteActionsController', data, function (res) {
                if (res) {
                    console.log('DeleteActionsController')
                }
            });
        }

        $($element).closest('tr').next().find('input[type="checkbox"][class="SelectedLocalPartition"]').prop('checked', $element.prop('checked'));
    });

function stateCheckedFormatter(value, row, index) {
    return row.state;
}