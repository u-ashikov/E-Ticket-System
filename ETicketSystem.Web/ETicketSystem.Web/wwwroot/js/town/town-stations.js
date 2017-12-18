$(function () {
            $('tr.all-stations').css('display', 'none');
            $('a.show-stations').click(function (event) {
                event.preventDefault();
                let trElement = event.target.parentElement.parentElement;
                let townId = $(trElement).find('td.townId').html();

                if ($(trElement).next('tr.all-stations').is(':hidden')) {
                    $('tr.all-stations').css('display', 'none');
                    $(trElement).next('tr.all-stations').show();
                }
                else {
                    $(trElement).next('tr.all-stations').css('display', 'none');
                }

                let url = $(this).attr('href');
                let infoContainer = $('tr.all-stations td ul.stations-list');

                $.ajax({
                    type: "GET",
                    url: url,
                    contentType: JSON,
                    data: { id: townId },
                    success: function (data) {
                        $(infoContainer).empty();
                        if (data.length == 0) {
                            $(infoContainer).append('<h3>No stations yet</h3>');
                        }
                        else {
                            for (var i = 0; i < data.length; i++) {
                                $(infoContainer).append(
                                    `<li class="station-info col-md-10">
                                        <div class="col-md-9">
                                            ${data[i].name}, <strong>Arriving routes:</strong> ${data[i].arrivingRoutes}, <strong>Departing routes:</strong> ${data[i].departingRoutes}
                                        </div>
                                        <a href="/admin/stations/edit/${data[i].id}" class="btn btn-primary col-md-1 edit-station">Edit</a>
                                </li>`);
                            }
                        }                       
                    }
                })
            })
        });