const themeMap = {
    dark: "light",
    light: "dark"
};

const theme = localStorage.getItem('theme')
    || (tmp = Object.keys(themeMap)[0],
        localStorage.setItem('theme', tmp),
        tmp);
const bodyClass = document.body.classList;
bodyClass.add(theme);

function toggleTheme() {
    const current = localStorage.getItem('theme');
    const next = themeMap[current];

    bodyClass.replace(current, next);
    localStorage.setItem('theme', next);

    ChangeChartFont()
}

function ChangeChartFont() {
    if (Chart) {
        Chart.defaults.global.defaultFontColor = bodyClass.contains('dark') ? 'white' : 'black';

        if (typeof FirstChart !== 'undefined') {
            FirstChart.update();
        }
        if (typeof SecondChart !== 'undefined') {
            SecondChart.update();
        }
        if (typeof PieChart !== 'undefined') {
            PieChart.update()
        }
        if (typeof BinChart !== 'undefined') {
            BinChart.update()
        }
    }
}

document.getElementById('themeButton').onclick = toggleTheme;