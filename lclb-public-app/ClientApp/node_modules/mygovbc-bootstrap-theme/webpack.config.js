var path = require('path');
const ExtractTextPlugin = require("extract-text-webpack-plugin");
const CleanWebpackPlugin = require('clean-webpack-plugin');
const CopyWebpackPlugin = require('copy-webpack-plugin');

const extractLess = new ExtractTextPlugin({
  filename: "mygovbc-bootstrap-theme.min.css",
  disable: process.env.NODE_ENV === "development"
});

module.exports = {
  target: 'web',
  entry: './src/index.less',
  output: {
    path: path.resolve(__dirname, 'dist'),
    filename: 'mygovbc-bootstrap-theme.min.css'
  },
  module: {
    rules: [{
        test: /\.less$/,
        use: extractLess.extract({
          use: [{
            loader: "css-loader",
            options: {
              minimize: true
            }
          }, {
            loader: "less-loader"
          }],
          // use style-loader in development
          fallback: "style-loader"
        })
      },
      {
        test: /\.(eot|svg|ttf|woff|woff2)$/,
        loader: 'file-loader?name=fonts/[name].[ext]'
      }
    ]
  },
  plugins: [
    new CleanWebpackPlugin(['dist'], {
      root: __dirname,
      verbose: true,
      dry: false
    }),
    extractLess,
    new CopyWebpackPlugin([{
        from: 'node_modules/bootstrap/dist/js',
        to: 'js'
      },
      {
        from: 'docs',
        to: ''
      },
      {
        from: 'src/images',
        to: 'images'
      }
    ])
  ]
};