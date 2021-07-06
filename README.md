# YoutubeMovieExtractor in csharp
This youtube movie extractor is build in june 2021 en has to be updated when Youtube is updating their code.

## General process
1) First it loads the raw html data from a video's default page.
2) From this raw html data is extracts the js player source.
3) With this source address the original js player can be downloaded.
4) The video info file has all the info about the different video file formats an must be downloaded separately.
5) With this video info file the data is extracted and put into VideoInfo instances.
6) From the video info file the ciphers are extracted.
7) The ciphers then are used for deciphering and placed at the end of the stream url.
8) With this url and the deciphered cipher the download and file saving can be done.

## Deciphering
1) From the js player first find the decipher function name.
2) With this name find the js function sequence (algorithm) based on the basic functions: swap, reverse and splice.
3) Map the found functions in the found sequence to the csharp equivalent.
4) Execute the decipher algorithm based on the found sequence.

## References
// https://gist.github.com/sidneys/7095afe4da4ae58694d128b1034e01e2
// https://www.youtube.com/get_video_info?video_id=tt66izA5y28
// https://tyrrrz.me/blog/reverse-engineering-youtube

// https://www.youtube.com/watch?v=8LKXQvvEOXE
// https://www.youtube.com/watch?v=uuPQZ0zhlPs

//			tag=43
//			type=video/webm; codecs="vp8.0, vorbis"
//			fallback_host=redirector.googlevideo.com
//			url=https://r12---sn-3c27sn7k.googlevideo.com/videoplayback?itag=43&lmt=1367519763212098&ipbits=0&key=yt6&mime=video%2Fwebm&expire=1511401259&mn=sn-3c27sn7k&mm=31&ms=au&mv=m&mt=1511379591&ei=y9IVWuuyKI-YdLvnm8AO&sparams=dur%2Cei%2Cgcr%2Cid%2Cinitcwndbps%2Cip%2Cipbits%2Citag%2Clmt%2Cmime%2Cmm%2Cmn%2Cms%2Cmv%2Cnh%2Cpl%2Cratebypass%2Crequiressl%2Csource%2Cexpire&ip=255.255.255.255&id=o-AJuM11wvxuVl2WBgfb3nr6zbmXsFGQvhMelDobZ_KOrE&nh=IgpwcjAxLmticDAxKgkxMjcuMC4wLjE&requiressl=yes&gcr=ua&source=youtube&ratebypass=yes&pl=24&initcwndbps=1112500&dur=0.000
//			s=9599599594B0133328AA570AE0129E58478D7BCE9D226F.15ABC404267945A3F64FB4E42074383FC4FA80F5
//			quality=medium