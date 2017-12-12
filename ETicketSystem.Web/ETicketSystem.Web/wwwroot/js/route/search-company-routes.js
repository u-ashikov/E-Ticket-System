function getSearchedRouteTowns(event) {
            let startTownId = $(event).parent().find('span.start-town-id').text();
            let endTownId = $(event).parent().find('span.end-town-id').text();

            $('form.search-form select.start-town').val(startTownId);
            $('form.search-form select.end-town').val(endTownId);

            $('form.search-form').submit();
}