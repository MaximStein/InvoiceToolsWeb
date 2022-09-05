const PROXY_CONFIG = [
  {
    context: [
      "/api",
    ],
    target: "https://localhost:7268",
    secure: false,
    "pathRewrite": {
      "^/api": ""
    },
    "logLevel": "debug"
  }
]

module.exports = PROXY_CONFIG;
