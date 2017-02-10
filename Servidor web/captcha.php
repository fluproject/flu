<?php
$ancho=100;
$alto=55;

//Colores
$image=imagecreatetruecolor($ancho,$alto);
$negro=imagecolorallocate($image,0,0,0);
$gray=imagecolorallocate($image,100,100,100);
$rgb[0] = rand(0,255);
$rgb[1] = rand(0,255);
$rgb[2] = rand(0,255);
$RandomColor=imagecolorallocate($image,$rgb[0],$rgb[1],$rgb[2]);
$RandomColorInverted=imagecolorallocate($image,255-$rgb[0],255-$rgb[1],255-$rgb[2]);

//Fondo
imagefill($image,0,0,$RandomColor);

//Marco
imageline($image,0,0,$ancho,0,$negro);
imageline($image,0,0,0,$alto,$negro);
imageline($image,$ancho-1,$alto-1,0,$alto-1,$negro);
imageline($image,$ancho-1,$alto-1,$ancho-1,0,$negro);

//Rejilla
imageline($image,25,0,25,$alto,$gray);
imageline($image,50,0,50,$alto,$gray);
imageline($image,75,0,75,$alto,$gray);
imageline($image,0,13,$ancho,13,$gray);
imageline($image,0,26,$ancho,26,$gray);
imageline($image,0,39,$ancho,39,$gray);

//TextoRandom
$random=substr(str_replace("0","",str_replace("O","",strtoupper(md5(rand(9999,99999))))),0,5);
//TextFont
$ttf = "fonts/gunplay.ttf";
imagefttext($image,22,rand(-10,15),12,37,$RandomColorInverted,$ttf,$random);

//Almacenamos el texto del captcha en la BBDD
error_reporting(0);
/*Se abre la conexion con la BBDD*/
include "conexion-bbdd.php";
$result=mysql_query("DELETE from t_captcha",$conexion);
$result=mysql_query("INSERT INTO t_captcha (captcha) VALUES ('" . $random . "')",$conexion);
/*Se cierra la conexión con la BBDD*/
include "cerrar-conexion-bbdd.php";

//Ruido
for ($i=0;$i<=700;$i++){
$randx=rand(0,100);
$randy=rand(0,55);
imagesetpixel($image,$randx,$randy,$RandomColorInverted);
}

//Salida
header("Content-type: image/png");
imagepng($image);
imagedestroy($image);
?>