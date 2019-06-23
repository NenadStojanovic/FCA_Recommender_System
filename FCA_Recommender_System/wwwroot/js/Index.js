$(document).ready(function () {
    $('#moviesTable').DataTable();
});


for (var i = 0; i < recomended.length; i++) {
    $.ajax({
        index: i,
        url: "https://api.themoviedb.org/3/search/movie",
        data: {
            api_key: "bfeeff106cd2938769ad5341b42337d3",
            region: 'US',
            query: recomended[i].name,
            page: 1
        },
        method: 'GET',
        dataType: 'jsonp',
        start_time: new Date().getTime(),
        success: function (response) {
            console.debug(response);
            var movies = response.results;
            var firstResult = movies[0];
            $("#logo_" + recomended[this.index].id).attr("src", "https://image.tmdb.org/t/p/w300/" + firstResult.poster_path)
        },
        error: function (response) {
            $("#logo_" + recomended[this.index].id).css("background: red;")
        }
    });
}