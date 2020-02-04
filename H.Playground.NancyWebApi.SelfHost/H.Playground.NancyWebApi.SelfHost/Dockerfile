FROM mono:6.8.0.96
RUN mkdir /app
COPY . /app
EXPOSE 8888
CMD ["mono", "/app/H.Playground.NancyWebApi.SelfHost.exe", "-d"]