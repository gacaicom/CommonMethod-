require.config({
    paths: {
        'geoJson': '../geoData/geoJson',
        'theme': '../echarts/theme',
        'data': '../Scripts/chart/data',
        'map': '../echarts/map',
        'extension': '../echarts/extension', 
        "jquery": "../Scripts/jQuery-2.1.4"

    },
    packages: [
        {
            main: 'echarts',
            location: '../echarts/src',
            name: 'echarts'
        },
        {
            main: 'zrender',
            location: '../echarts/zrender/src',
            name: 'zrender'
        } 
    ]
    // urlArgs: '_v_=' + +new Date()
});