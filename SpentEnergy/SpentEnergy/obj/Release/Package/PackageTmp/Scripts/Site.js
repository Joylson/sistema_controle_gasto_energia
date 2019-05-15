

let months = [
    "Jan",
    "Fev",
    "Mar",
    "Abr",
    "Mai",
    "Jun",
    "Jul",
    "Ago",
    "Set",
    "Out",
    "Nov",
    "Dez"
];

let hours = [
    "0 AM",
    "1 AM",
    "2 AM",
    "3 AM",
    "4 AM",
    "5 AM",
    "6 AM",
    "7 AM",
    "8 AM",
    "9 AM",
    "10 AM",
    "11 AM",
    "12 AM",
    "1 PM",
    "2 PM",
    "3 PM",
    "4 PM",
    "5 PM",
    "6 PM",
    "7 PM",
    "8 PM",
    "9 PM",
    "10 PM",
    "11 PM"
];

let atulization_global = true;



let alterType = function (type) {
    if (type === 'D') {
        return createSeq(31, 'DIA');
    } else if (type === 'M') {
        return months;
    } else if (type === 'H') {
        return hours;
    }
};

let createSeq = function (range, desc) {
    let seq = [];
    for (let i = 1; i <= range; i++) {
        seq[i - 1] = i + ' ' + desc;
    }
    return seq;
};


let chackAlter = function (chart1, chart2) {
    let atua = chart1 === [] || chart2 === [] ? true : false;
    atua = chart1.length !== chart2.length;

    for (let x = 0; x < chart1.length; x++) {
        for (let y = 0; y < chart2.length; y++) {
            if (chart2[y]['id'] === chart1[x]['id'] && chart1[x]['data'].length > 0 && chart2[y]['data'].length > 0) {
                for (let i = 0; i < chart1[x]['data'].length; i++) {
                    if (chart2[y]['data'][i] !== chart1[x]['data'][i] && chart2[y]['data'][i] !== undefined && chart1[x]['data'][i] !== undefined) {
                        atua = true;
                    }
                }
            }
        }
    }
    return atua;
};

random_color = function (min_alpha) {
    min_alpha = min_alpha | 20;
    r = Math.floor(Math.random() * (255 - 1) + 1);
    g = Math.floor(Math.random() * (255 - 1) + 1);
    b = Math.floor(Math.random() * (255 - 1) + 1);
    a = ((Math.random() * (100 - min_alpha) + min_alpha) / 100).toFixed(2);
    return 'rgba(' + r + ', ' + g + ', ' + b + ', ' + a + ')';
};


random_colors = function (size, min_alpha) {
    colors = Array(size);
    for (let i = 0; i < size; i++) {
        let color = random_color(min_alpha);
        if (colors.includes(color)) {
            i--;
        } else {
            colors[i] = color;
        }
    }
    return colors;
};

let toggleAtu = function () {
    if (atulization_global) {
        atulization_global = false;
        $('#btn-toggle').removeClass('btn-success');
        $('#btn-toggle').addClass('btn-danger');
    } else {
        atulization_global = true;
        $('#btn-toggle').removeClass('btn-danger');
        $('#btn-toggle').addClass('btn-success');
    }
};