﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCVenta.Models;
using MVCVenta.ViewModels;

namespace MVCVenta.Controllers
{
    public class CarritoComprasController : Controller
    {
        //
        // GET: /CarritoCompras/

            public readonly VentaDataClassesDataContext _data;

        public CarritoComprasController(VentaDataClassesDataContext data)
        {
            _data = data;
        }

        public CarritoComprasController()
        {
            _data = new VentaDataClassesDataContext();
        }

      
        public ActionResult Index(int id)
        {
            List<CarritoCompra> listaCarritoCompras = null;
            //Si no escogido ningun producto
            if (id != 0)
            {
                CarritoCompra objCarrito = new CarritoCompra();

                var productoAgregado = _data.TB_Productos
                   .Single(Id => Id.Pk_eProducto == id);


                if (Session["Carrito"] != null)
                {
                    listaCarritoCompras = (List<CarritoCompra>)Session["Carrito"];
                    objCarrito.IdProducto = productoAgregado.Pk_eProducto;
                    objCarrito.DescripcionProducto = productoAgregado.cDescripcion;
                    objCarrito.PrecioProducto = productoAgregado.dPrecio;
                    objCarrito.CantProducto = 1;
                    objCarrito.TotalProducto = productoAgregado.dPrecio;
                    listaCarritoCompras.Add(objCarrito);
                    Session["Carrito"] = listaCarritoCompras;
                }
                else
                {
                    objCarrito.IdProducto = productoAgregado.Pk_eProducto;
                    objCarrito.DescripcionProducto = productoAgregado.cDescripcion;
                    objCarrito.PrecioProducto = productoAgregado.dPrecio;
                    objCarrito.CantProducto = 1;
                    objCarrito.TotalProducto = productoAgregado.dPrecio;
                    listaCarritoCompras = new List<CarritoCompra>();
                    listaCarritoCompras.Add(objCarrito);
                    Session["Carrito"] = listaCarritoCompras;

                }
            }
            else {
                listaCarritoCompras = (List<CarritoCompra>)Session["Carrito"];
            }

            return View(listaCarritoCompras);
           // return View();
        }

        //
        // GET: /CarritoCompras/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }


        //public ActionResult Cancelar()
        //{
        //    //listaCarritoCompras = null;
        //    Session["Carrito"] = null;
        //    return RedirectToAction("Index/1","Home");
        //}

        //  [AcceptVerbs(HttpVerbs.Post)]

        //public ActionResult Guardar(FormCollection collection)
        //{

        //[HttpPost]
        //[ActionName("Post")]
        //[AcceptSubmitButton(Name = "btnGuardar", Value = "Añadir otro producto")]
        public ActionResult Guardar(FormCollection fc)
        {
            //Guardar el producto seleccionado
            if (fc["btnGuardar"] != null)
            {
                List<CarritoCompra> listaCarritoCompras = null;
                if (Session["Carrito"] != null)
                {
                    listaCarritoCompras = (List<CarritoCompra>)Session["Carrito"];
                    string[] Cantidades = Request.Form["txtCantProducto"].Split(char.Parse(","));
                    string[] TotalesProducto = Request.Form["hdTotalProducto"].Split(char.Parse(","));

                    for (int i = 0; i < Cantidades.Length; i++)
                    {
                        listaCarritoCompras[i].CantProducto = int.Parse(Cantidades[i].ToString());
                        listaCarritoCompras[i].TotalProducto = decimal.Parse(TotalesProducto[i].ToString());
                    }

                    Session["Carrito"] = listaCarritoCompras;
                }


            }
                //Realizar la compra
            else if (fc["btnCompra"] != null)
            {
                //List<CarritoCompra> listaCarritoCompras = null;
                //if (Session["Carrito"] != null)
                //{
                //    listaCarritoCompras = (List<CarritoCompra>)Session["Carrito"];
                //    //Guardar en base de datos

                //    Session["Carrito"] = null;

                //    return RedirectToAction("LogOn", "Account");
                //}
                return RedirectToAction("LogOn", "Account");

            }
                //Cancelar la compra
            else if (fc["btnCancelar"] != null) {
                Session["Carrito"] = null;
                //return RedirectToAction("Index/1", "Home");
            
            }
          
            return RedirectToAction("Index/1", "Home");
        }

        //
        // GET: /CarritoCompras/Create

        public ActionResult RealizarCompra()
        {
            List<CarritoCompra> listaCarritoCompras = null;
            //if (Session["Carrito"] != null)
            //{
            string rpta = string.Empty;
                listaCarritoCompras = (List<CarritoCompra>)Session["Carrito"];

            //    Decimal dMonto = listaCarritoCompras.Sum(P => P.CantProducto * P.PrecioProducto);
                Decimal dMonto = listaCarritoCompras.Sum(P => P.TotalProducto);
                //Guardar en base de datos
                WsBanco.NroCuentaServiceClient oWsBancoJava = new WsBanco.NroCuentaServiceClient();
                //rpta = oWsBancoJava.obtenerTransaccion("6958474589632458", "0201", "SOL", 100);
                rpta = oWsBancoJava.obtenerTransaccion("6958474589632458", "0201", "SOL", (double)dMonto);

                if (rpta != "-1")
                {
                    //Limpiar
                    Session["Carrito"] = null;
                    ViewData["Message"] = string.Format("La compra se realizo correctamente,<br/> El Monto total fue de : {0}", dMonto.ToString());
                }
                else
                {
                    ViewData["Message"] = "Ocurrió un erro en la transacción.";
                }
                ViewData["Message"] = "La compra se realizo correctamente";
               // return RedirectToAction("Index/0", "CarritoCompras");
                return View();
            //}
            
        } 

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /CarritoCompras/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        
        //
        // GET: /CarritoCompras/Edit/5
 
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /CarritoCompras/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /CarritoCompras/Delete/5
 
        public ActionResult Delete(int id)
        {

            List<CarritoCompra> listaCarritoCompras = null;
            if (Session["Carrito"] != null)
            {
                listaCarritoCompras = (List<CarritoCompra>)Session["Carrito"];

                //Buscar un objeto Carrito
                CarritoCompra itemCarrito = listaCarritoCompras.Find(delegate(CarritoCompra c) {return c.IdProducto == id; });
                listaCarritoCompras.Remove(itemCarrito);

                Session["Carrito"] = listaCarritoCompras;
                //Si al eliminar los productos manualmente no hay productos regresamos a la pagina de catalogo de productos
                if (listaCarritoCompras.Count == 0) {
                    return RedirectToAction("Index/1", "Home");
                }
            }

            //return View(listaCarritoCompras);
            return RedirectToAction("Index/0", "CarritoCompras");
           
        }

        //
        // POST: /CarritoCompras/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
